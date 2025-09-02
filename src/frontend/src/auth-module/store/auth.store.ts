import { makeAutoObservable, runInAction } from "mobx";

import { HttpClient } from "src/shared-module/api";
import { authApiService, type LoginRequest } from "../api";
import { type UserModel } from "../models";

class AuthStore {
  private _isLogged: boolean | null = localStorage.getItem("token") !== null;
  private _userId: number | null = null;

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

  public get userId(): number | null {
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
}

export const authStore = new AuthStore();
