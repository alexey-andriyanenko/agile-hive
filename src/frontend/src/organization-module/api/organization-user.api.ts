import { appHttpClient } from "src/shared-module/api";
import type {
  CreateOrganizationUserRequest,
  GetManyOrganizationUsersByIdsRequest,
  GetManyOrganizationUsersByIdsResponse,
  GetManyOrganizationUsersRequest,
  GetOrganizationUserByIdRequest,
  GetOrganizationUserByIdResponse,
  UpdateOrganizationUserRequest,
} from "./organization-user.types.ts";

class OrganizationUserApiService {
  getOrganizationUserById(data: GetOrganizationUserByIdRequest) {
    return appHttpClient
      .get<GetOrganizationUserByIdResponse>(
        `/organizations/${data.organizationId}/users/${data.id}`,
      )
      .send();
  }

  getManyOrganizationUsersByIds(data: GetManyOrganizationUsersByIdsRequest) {
    return appHttpClient
      .get<GetManyOrganizationUsersByIdsResponse>(
        `/organizations/${data.organizationId}/users/by-ids`,
      )
      .setSearchParams({
        userIds: data.ids,
      })
      .send();
  }

  getManyOrganizationUsers(data: GetManyOrganizationUsersRequest) {
    return appHttpClient
      .get<GetManyOrganizationUsersByIdsResponse>(`/organizations/${data.organizationId}/users`)
      .send();
  }

  createOrganizationUser(data: CreateOrganizationUserRequest) {
    return appHttpClient
      .post<
        CreateOrganizationUserRequest,
        GetOrganizationUserByIdResponse
      >(`/organizations/${data.organizationId}/users`)
      .send(data);
  }

  updateOrganizationUser(data: UpdateOrganizationUserRequest) {
    return appHttpClient
      .put<
        UpdateOrganizationUserRequest,
        GetOrganizationUserByIdResponse
      >(`/organizations/${data.organizationId}/users/${data.id}`)
      .send(data);
  }

  removeOrganizationUser(data: GetOrganizationUserByIdRequest) {
    return appHttpClient
      .delete<void>(`/organizations/${data.organizationId}/users/${data.id}`)
      .send();
  }

  removeManyOrganizationUsers(data: GetManyOrganizationUsersByIdsRequest) {
    return appHttpClient
      .delete<void>(`/organizations/${data.organizationId}/users`)
      .setSearchParams({
        userIds: data.ids,
      })
      .send();
  }
}

export const organizationUserApiService = new OrganizationUserApiService();
