import { HttpClient } from "src/shared-module/api/http-client/http-client";
import { authStore } from "src/auth-module/store/auth.store.ts";

export const appHttpClient = new HttpClient({
  baseUrl: "http://localhost:5200/api/v1",
  interceptors: [
    async (request: XMLHttpRequest) => {
      if (request.status === 401 && window.location.pathname !== "/auth/login") {
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

export { HttpClient };
