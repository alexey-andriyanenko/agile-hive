import { makeAutoObservable, runInAction } from "mobx";
import type { OrganizationUserModel } from "../models/organization-user.ts";
import {
  type CreateOrganizationUserRequest,
  type GetManyOrganizationUsersByIdsRequest,
  type GetManyOrganizationUsersRequest,
  type GetOrganizationUserByIdRequest,
  organizationUserApiService,
  type RemoveOrganizationUserRequest,
  type UpdateOrganizationUserRequest,
} from "src/organization-module/api";

class OrganizationUsersStore {
  private _users: OrganizationUserModel[] = [];

  public get users(): OrganizationUserModel[] {
    return this._users;
  }

  constructor() {
    makeAutoObservable(this);
  }

  public async fetchUserById({
    organizationId,
    id,
  }: GetOrganizationUserByIdRequest): Promise<void> {
    const res = await organizationUserApiService.getOrganizationUserById({
      organizationId,
      id,
    });
    const index = this._users.findIndex((user) => user.id === res.id);

    if (index !== -1) {
      runInAction(() => {
        this._users[index] = res;
      });
    }
  }

  public async fetchManyUsersByIds({
    organizationId,
    ids,
  }: GetManyOrganizationUsersByIdsRequest): Promise<void> {
    const res = await organizationUserApiService.getManyOrganizationUsersByIds({
      organizationId,
      ids,
    });

    for (const user of res.users) {
      const index = this._users.findIndex((u) => u.id === user.id);

      if (index !== -1) {
        runInAction(() => {
          this._users[index] = user;
        });
      }
    }
  }

  public async fetchManyUsers({ organizationId }: GetManyOrganizationUsersRequest): Promise<void> {
    const res = await organizationUserApiService.getManyOrganizationUsers({ organizationId });
    runInAction(() => {
      this._users = res.users;
    });
  }

  public async createUser(data: CreateOrganizationUserRequest): Promise<void> {
    const res = await organizationUserApiService.createOrganizationUser(data);

    runInAction(() => {
      this._users.push(res);
    });
  }

  public async updateUser(data: UpdateOrganizationUserRequest): Promise<void> {
    const res = await organizationUserApiService.updateOrganizationUser(data);
    const index = this._users.findIndex((user) => user.id === res.id);

    if (index !== -1) {
      runInAction(() => {
        this._users[index] = res;
      });
    }
  }

  public async removeUser(data: RemoveOrganizationUserRequest): Promise<void> {
    await organizationUserApiService.removeOrganizationUser(data);
    const index = this._users.findIndex((user) => user.id === data.id);

    if (index !== -1) {
      runInAction(() => {
        this._users.splice(index, 1);
      });
    }
  }

  public async removeManyUsers(organizationId: string, ids: string[]): Promise<void> {
    await organizationUserApiService.removeManyOrganizationUsers({ organizationId, ids });

    runInAction(() => {
      this._users = this._users.filter((user) => !ids.includes(user.id));
    });
  }
}

export const organizationUserStore = new OrganizationUsersStore();
