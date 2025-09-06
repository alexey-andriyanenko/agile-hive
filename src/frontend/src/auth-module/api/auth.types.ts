export type LoginRequest = {
  email: string;
  password: string;
};

export interface LoginResponse {
  userId: number;
  accessToken: string;
  refreshToken: string;
}

export type RegisterRequest = {
  firstName: string;
  lastName: string;
  userName: string;
  email: string;
  password: string;
  organizationName?: string;
};

export type RegisterResponse = {
  userId: string;
  accessToken: string;
  refreshToken: string;
};

export type RefreshTokenRequest = {
  refreshToken: string;
};

export type RefreshTokenResponse = {
  accessToken: string;
  refreshToken: string;
};
