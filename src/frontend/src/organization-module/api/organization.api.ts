import { appHttpClient } from "src/shared-module/api";
import type {
  CreateOrganizationRequest,
  CreateOrganizationResponse,
  GetOrganizationsResponse,
} from "./organization.types.ts";

class OrganizationApiService {
  getOrganizations() {
    return appHttpClient.get<GetOrganizationsResponse>("/api/core/organizations").send();
  }

  createOrganization(data: CreateOrganizationRequest) {
    return appHttpClient
      .post<CreateOrganizationRequest, CreateOrganizationResponse>("/api/core/organizations")
      .send(data);
  }
}

export const organizationApiService = new OrganizationApiService();
