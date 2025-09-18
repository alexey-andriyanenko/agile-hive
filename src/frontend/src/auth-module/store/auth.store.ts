import { makeAutoObservable, runInAction } from "mobx";

import { HttpClient } from "src/shared-module/api";
import { authApiService, type LoginRequest } from "../api";
import { type UserModel } from "../models";

class AuthStore {
  private _isLogged: boolean | null = localStorage.getItem("token") !== null;
  private _userId: string | null = null;

  private _currentUser: UserModel | null = null;

  constructor() {
    makeAutoObservable(this);
  }

  public get isLogged() {
    return this._isLogged;
  }

  public get currentUser(): UserModel | null {
    return this._currentUser;
  }

  public get userId(): string | null {
    return this._userId;
  }

  async signIn(data: LoginRequest): Promise<void> {
    const res = await authApiService.login(data);
    localStorage.setItem("accessToken", res.accessToken);
    localStorage.setItem("refreshToken", res.refreshToken);

    runInAction(() => {
      this._isLogged = true;

      HttpClient.accessToken = res.accessToken;
      HttpClient.refreshToken = res.refreshToken;
    });
  }

  async signInWithRefreshToken(): Promise<string> {
    const refreshToken = localStorage.getItem("refreshToken");

    if (!refreshToken) {
      throw new Error("No refresh token found");
    }

    const res = await authApiService.loginWithRefreshToken({
      refreshToken,
    });

    localStorage.setItem("accessToken", res.accessToken);
    localStorage.setItem("refreshToken", res.refreshToken);

    runInAction(() => {
      this._isLogged = true;

      HttpClient.accessToken = res.accessToken;
      HttpClient.refreshToken = res.refreshToken;
    });

    return res.accessToken;
  }

  async loadMe(): Promise<void> {
    try {
      const res = await authApiService.getMe();

      runInAction(() => {
        this._currentUser = res;
        this._userId = res.id;
        this._isLogged = true;
      });
    } catch (error) {
      runInAction(() => {
        this._isLogged = false;
        this._currentUser = null;
        this._userId = null;
      });

      throw error;
    }
  }
}

export const authStore = new AuthStore();
