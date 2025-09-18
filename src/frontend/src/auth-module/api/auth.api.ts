import { appHttpClient } from "src/shared-module/api";

import type {
  LoginRequest,
  LoginResponse,
  RefreshTokenRequest,
  RefreshTokenResponse,
  RegisterRequest,
  RegisterResponse,
} from "./auth.types";
import type { UserModel } from "src/auth-module/models";

class AuthApiService {
  login(data: LoginRequest) {
    return appHttpClient.post<LoginRequest, LoginResponse>("/identity/login").send(data);
  }

  loginWithRefreshToken(data: RefreshTokenRequest) {
    return appHttpClient
      .post<RefreshTokenRequest, RefreshTokenResponse>("/token/refresh")
      .send(data);
  }

  register(data: RegisterRequest) {
    return appHttpClient.post<RegisterRequest, RegisterResponse>("/identity/register").send(data);
  }

  getMe() {
    return appHttpClient.get<UserModel>("/user/me").send();
  }
}

export const authApiService = new AuthApiService();
