import { makeAutoObservable, runInAction } from "mobx";
import type { OrganizationModel } from "../models/organization.ts";
import { organizationApiService } from "../api/organization.api.ts";

class OrganizationStore {
  private _currentOrganization: OrganizationModel | null = null;

  private _organizations: OrganizationModel[] = [];

  public get currentOrganization(): OrganizationModel | null {
    return this._currentOrganization;
  }

  public get organizations(): OrganizationModel[] {
    return this._organizations;
  }

  constructor() {
    makeAutoObservable(this);
  }

  public setCurrentOrganization(organization: OrganizationModel): void {
    runInAction(() => {
      this._currentOrganization = organization;
    });
  }

  public async fetchOrganizations(): Promise<void> {
    const res = await organizationApiService.getOrganizations();

    runInAction(() => {
      this._organizations = res;
    });
  }

  public async createOrganization(data: { name: string; description: string }): Promise<void> {
    const res = await organizationApiService.createOrganization(data);

    runInAction(() => {
      this._organizations.push(res);
    });
  }
}

export const organizationStore = new OrganizationStore();
