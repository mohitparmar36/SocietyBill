import React, { useEffect } from "react";
import { useAuth0 } from "@auth0/auth0-react";
import api from "./axios";

export const AxiosInterceptor = ({ children }: { children: React.ReactNode }) => {
  const { getAccessTokenSilently } = useAuth0();

  useEffect(() => {
    const requestInterceptor = api.interceptors.request.use(async (config) => {
      try {
        const token = await getAccessTokenSilently();
        if (token) {
          config.headers["Authorization"] = `Bearer ${token}`;
        }
      } catch (error) {
        console.error("Error getting access token", error);
      }
      return config;
    });

    return () => {
      api.interceptors.request.eject(requestInterceptor);
    };
  }, [getAccessTokenSilently]);

  return <>{children}</>;
};
