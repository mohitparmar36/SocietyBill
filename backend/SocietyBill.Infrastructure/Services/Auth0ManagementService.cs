using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Configuration;
using SocietyBill.Application.Interfaces.Services;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SocietyBill.Infrastructure.Services
{
    public class Auth0ManagementService : IAuth0ManagementService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public Auth0ManagementService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            var domain = _configuration["Auth0:Domain"];
            var clientId = _configuration["Auth0:ManagementClientId"];
            var clientSecret = _configuration["Auth0:ManagementClientSecret"];

            var request = new HttpRequestMessage(HttpMethod.Post, $"https://{domain}/oauth/token");
            request.Content = new StringContent(JsonSerializer.Serialize(new
            {
                client_id = clientId,
                client_secret = clientSecret,
                audience = $"https://{domain}/api/v2/",
                grant_type = "client_credentials"
            }), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var tokenResponse = JsonSerializer.Deserialize<JsonElement>(content);
            return tokenResponse.GetProperty("access_token").GetString()!;
        }

        private async Task AssignRoleToUserAsync(ManagementApiClient client, string auth0UserId, string roleName, CancellationToken cancellationToken)
        {
            try
            {
                // First get the role ID by name
                var rolesList = await client.Roles.GetAllAsync(new GetRolesRequest { NameFilter = roleName }, cancellationToken: cancellationToken);
                var role = System.Linq.Enumerable.FirstOrDefault(rolesList, r => r.Name == roleName);

                string roleId;
                if (role != null)
                {
                    roleId = role.Id;
                }
                else
                {
                    // Create it if it doesn't exist
                    var newRole = await client.Roles.CreateAsync(new RoleCreateRequest { Name = roleName, Description = $"Auto-generated {roleName} role" }, cancellationToken);
                    roleId = newRole.Id;
                }

                // Assign the role to the user
                await client.Users.AssignRolesAsync(auth0UserId, new AssignRolesRequest { Roles = new[] { roleId } }, cancellationToken);
            }
            catch (Exception ex)
            {
                // Log exception if role assignment fails (e.g., missing scopes in M2M app)
                Console.WriteLine($"Warning: Failed to assign RBAC role {roleName} to user {auth0UserId}: {ex.Message}");
            }
        }

        public async Task<string> CreateUserAsync(string email, string password, Guid societyId, Guid flatId, CancellationToken cancellationToken = default)
        {
            var domain = _configuration["Auth0:Domain"];
            var token = await GetAccessTokenAsync(cancellationToken);
            using var client = new ManagementApiClient(token, new Uri($"https://{domain}/api/v2/"));

            var userCreateRequest = new UserCreateRequest
            {
                Email = email,
                Password = password,
                Connection = "SocietyBillDB",
                VerifyEmail = false, // We will trigger the password reset instead
                AppMetadata = new
                {
                    societyId = societyId.ToString(),
                    flatId = flatId.ToString(),
                    roles = new[] { "Resident" } // Keep it in app_metadata as a fallback/quick-access
                }
            };

            var user = await client.Users.CreateAsync(userCreateRequest, cancellationToken);

            // Assign RBAC Role via Auth0 post-user-roles API
            await AssignRoleToUserAsync(client, user.UserId, "Resident", cancellationToken);

            return user.UserId;
        }

        public async Task UpdateUserAppMetadataAsync(string auth0UserId, Guid societyId, CancellationToken cancellationToken = default)
        {
            var domain = _configuration["Auth0:Domain"];
            var token = await GetAccessTokenAsync(cancellationToken);
            using var client = new ManagementApiClient(token, new Uri($"https://{domain}/api/v2/"));

            var userUpdateRequest = new UserUpdateRequest
            {
                AppMetadata = new
                {
                    societyId = societyId.ToString(),
                    roles = new[] { "SocietyAdmin" }
                }
            };

            await client.Users.UpdateAsync(auth0UserId, userUpdateRequest, cancellationToken);

            // Assign RBAC Role via Auth0 post-user-roles API
            await AssignRoleToUserAsync(client, auth0UserId, "SocietyAdmin", cancellationToken);
        }

        public async Task<string> CreatePasswordResetTicketAsync(string auth0UserId, CancellationToken cancellationToken = default)
        {
            var domain = _configuration["Auth0:Domain"];
            var clientId = _configuration["Auth0:ManagementClientId"];

            // We first need the user's email to trigger the email reset
            var token = await GetAccessTokenAsync(cancellationToken);
            using var mClient = new ManagementApiClient(token, new Uri($"https://{domain}/api/v2/"));
            var user = await mClient.Users.GetAsync(auth0UserId, cancellationToken: cancellationToken);

            using var httpClient = new HttpClient();
            var payload = new
            {
                client_id = clientId,
                email = user.Email,
                connection = "SocietyBillDB"
            };

            var response = await httpClient.PostAsJsonAsync($"https://{domain}/dbconnections/change_password", payload, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return "Auth0 has successfully dispatched the Password Reset email.";
            }

            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"Failed to trigger Auth0 password reset email: {error}");
        }
    }
}
