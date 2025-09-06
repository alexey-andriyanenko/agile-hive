import { HttpClient } from "./http-client";
import { authStore } from "src/auth-module/store/auth.store.ts";

export { HttpClient };

export const httpClient = new HttpClient({
  baseUrl: "http://localhost:8000",
  interceptors: [
    async (request: XMLHttpRequest) => {
      if (request.status === 401) {
        const newAccessToken = await authStore.signInWithRefreshToken();

        if (newAccessToken) {
          request.setRequestHeader("Authorization", `Bearer ${newAccessToken}`);
          request.send();
          return;
        }

        localStorage.removeItem("accessToken");
        localStorage.removeItem("refreshToken");
        window.location.href = "/auth/login";
      }
    },
  ],
});
