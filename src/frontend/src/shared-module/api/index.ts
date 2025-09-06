import { HttpClient } from "src/shared-module/api/http-client";

export const appHttpClient = new HttpClient({
  baseUrl: "http://localhost:5200/api/v1",
  interceptors: [
    (request: XMLHttpRequest) => {
      if (request.status === 401 || request.status === 403) {
        localStorage.removeItem("accessToken");
        localStorage.removeItem("refreshToken");
        window.location.href = "/auth/login";
      }
    },
  ],
});

export { HttpClient };
