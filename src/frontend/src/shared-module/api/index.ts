import { HttpClient } from "src/shared-module/api/http-client";

export const appHttpClient = new HttpClient("http://localhost:5174/api/v1");
export { HttpClient };
