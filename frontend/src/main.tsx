import React from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import { Auth0Provider } from "@auth0/auth0-react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { AxiosInterceptor } from "./api/AxiosInterceptor";
import App from "./App";
import "../index.css";

const queryClient = new QueryClient();

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <Auth0Provider
      domain={import.meta.env.VITE_AUTH0_DOMAIN}
      clientId={import.meta.env.VITE_AUTH0_CLIENT_ID}
      authorizationParams={{
        redirect_uri: window.location.origin,
        audience: import.meta.env.VITE_AUTH0_AUDIENCE,
        scope: "openid profile email",
      }}
      cacheLocation="localstorage"
    >
      <QueryClientProvider client={queryClient}>
        <AxiosInterceptor>
          <BrowserRouter>
            <App />
          </BrowserRouter>
        </AxiosInterceptor>
      </QueryClientProvider>
    </Auth0Provider>
  </React.StrictMode>
);
