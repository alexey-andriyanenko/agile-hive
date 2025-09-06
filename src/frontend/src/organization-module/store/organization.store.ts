import { makeAutoObservable, runInAction } from "mobx";
import type { OrganizationModel } from "../models/organization.ts";
import { organizationApiService } from "../api/organization.api.ts";
import type { CreateOrganizationRequest } from "src/organization-module/api";

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
      this._organizations = res.organizations;
    });
  }

  public async fetchCurrentOrganizationBySlug(slug: string): Promise<void> {
    const res = await organizationApiService.getOrganizationBySlug(slug);

    runInAction(() => {
      this._currentOrganization = res;
    });
  }

  public async createOrganization(data: CreateOrganizationRequest): Promise<void> {
    const res = await organizationApiService.createOrganization(data);

    runInAction(() => {
      this._organizations.push(res);
    });
  }
}

export const organizationStore = new OrganizationStore();
