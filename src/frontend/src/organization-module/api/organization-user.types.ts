import type { OrganizationUserModel } from "src/organization-module/models/organization-user.ts";
import type { OrganizationUserRole } from "src/organization-module/models/organization-user-role.ts";

export type GetOrganizationUserByIdRequest = {
  organizationId: string;
  id: string;
};

export type GetOrganizationUserByIdResponse = OrganizationUserModel;

export type GetManyOrganizationUsersByIdsRequest = {
  organizationId: string;
  ids: string[];
};

export type GetManyOrganizationUsersByIdsResponse = {
  users: OrganizationUserModel[];
};

export type GetManyOrganizationUsersRequest = {
  organizationId: string;
};

export type GetManyOrganizationUsersResponse = {
  users: OrganizationUserModel[];
};

export type CreateOrganizationUserRequest = {
  organizationId: string;
  firstName: string;
  lastName: string;
  email: string;
  userName: string;
  password: string;
  role: OrganizationUserRole;
};

export type CreateOrganizationUserResponse = OrganizationUserModel;

export type UpdateOrganizationUserRequest = CreateOrganizationUserRequest & {
  id: string;
};

export type UpdateOrganizationUserResponse = OrganizationUserModel;

export type RemoveOrganizationUserRequest = {
  organizationId: string;
  id: string;
};

export type RemoveManyOrganizationUsersRequest = {
  organizationId: string;
  ids: string[];
};
