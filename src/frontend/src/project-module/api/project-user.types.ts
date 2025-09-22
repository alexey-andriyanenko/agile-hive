import { type ProjectUserModel, ProjectUserRole } from "src/project-module/models";

export type GetProjectUserByIdRequest = {
  organizationId: string;
  projectId: string;
  id: string;
};

export type GetProjectUserByIdResponse = ProjectUserModel;

export type GetManyProjectUsersByIdsRequest = {
  organizationId: string;
  projectId: string;
  ids: string[];
};

export type GetManyProjectUsersByIdsResponse = {
  users: ProjectUserModel[];
};

export type GetManyProjectUsersRequest = {
  organizationId: string;
  projectId: string;
};

export type GetManyProjectUsersResponse = {
  users: ProjectUserModel[];
};

export type AddUserToProjectRequest = {
  organizationId: string;
  projectId: string;
  userId: string;
  role: ProjectUserRole;
};

export type AddUserToProjectResponse = ProjectUserModel;

export type AddManyUsersToProjectRequest = {
  organizationId: string;
  projectId: string;
  users: AddUserToProjectItem[];
};

export type AddUserToProjectItem = {
  userId: string;
  role: ProjectUserRole;
};

export type AddManyUsersToProjectResponse = {
  users: ProjectUserModel[];
};

export type UpdateProjectUserRequest = {
  organizationId: string;
  projectId: string;
  id: string;
  role: ProjectUserRole;
};

export type UpdateProjectUserResponse = ProjectUserModel;

export type RemoveProjectUserRequest = {
  organizationId: string;
  projectId: string;
  id: string;
};

export type RemoveManyProjectUsersRequest = {
  organizationId: string;
  projectId: string;
  ids: string[];
};
