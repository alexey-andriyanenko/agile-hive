import { appHttpClient } from "src/shared-module/api";
import type {
  AddManyUsersToProjectRequest,
  AddManyUsersToProjectResponse,
  AddUserToProjectRequest,
  AddUserToProjectResponse,
  GetManyProjectUsersByIdsRequest,
  GetManyProjectUsersByIdsResponse,
  GetManyProjectUsersRequest,
  GetManyProjectUsersResponse,
  GetProjectUserByIdRequest,
  GetProjectUserByIdResponse,
  RemoveManyProjectUsersRequest,
  RemoveProjectUserRequest,
  UpdateProjectUserRequest,
  UpdateProjectUserResponse,
} from "src/project-module/api/project-user.types.ts";

class ProjectUserApiService {
  getProjectUserById(data: GetProjectUserByIdRequest) {
    return appHttpClient
      .get<GetProjectUserByIdResponse>(
        "/organization/:organizationId/projects/:projectId/users/:userId",
      )
      .setRouteParams({
        organizationId: data.organizationId,
        projectId: data.projectId,
        userId: data.id,
      })
      .send();
  }

  getManyProjectUsersByIds(data: GetManyProjectUsersByIdsRequest) {
    return appHttpClient
      .post<GetManyProjectUsersByIdsRequest, GetManyProjectUsersByIdsResponse>(
        "/organization/:organizationId/projects/:projectId/users",
      )
      .setRouteParams({
        organizationId: data.organizationId,
        projectId: data.projectId,
      })
      .setSearchParams({
        userIds: data.ids,
      })
      .send();
  }

  getManyProjectUsers(data: GetManyProjectUsersRequest) {
    return appHttpClient
      .get<GetManyProjectUsersResponse>("/organization/:organizationId/projects/:projectId/users")
      .setRouteParams({
        organizationId: data.organizationId,
        projectId: data.projectId,
      })
      .send();
  }

  addUserToProject(data: AddUserToProjectRequest) {
    return appHttpClient
      .post<AddUserToProjectRequest, AddUserToProjectResponse>(
        "/organization/:organizationId/projects/:projectId/users/add",
      )
      .setRouteParams({
        organizationId: data.organizationId,
        projectId: data.projectId,
      })
      .send(data);
  }

  addManyUsersToProject(data: AddManyUsersToProjectRequest) {
    return appHttpClient
      .post<AddManyUsersToProjectRequest, AddManyUsersToProjectResponse>(
        "/organization/:organizationId/projects/:projectId/users/many",
      )
      .setRouteParams({
        organizationId: data.organizationId,
        projectId: data.projectId,
      })
      .send(data);
  }

  updateProjectUser(data: UpdateProjectUserRequest) {
    return appHttpClient
      .put<UpdateProjectUserRequest, UpdateProjectUserResponse>(
        "/organization/:organizationId/projects/:projectId/users/:userId",
      )
      .setRouteParams({
        organizationId: data.organizationId,
        projectId: data.projectId,
        userId: data.id,
      })
      .send(data);
  }

  removeProjectUser(data: RemoveProjectUserRequest): Promise<void> {
    return appHttpClient
      .delete<void>("/organization/:organizationId/projects/:projectId/users/:userId")
      .setRouteParams({
        organizationId: data.organizationId,
        projectId: data.projectId,
        userId: data.id,
      })
      .send();
  }

  removeManyProjectUsers(data: RemoveManyProjectUsersRequest): Promise<void> {
    return appHttpClient
      .delete<void>("/organization/:organizationId/projects/:projectId/users")
      .setRouteParams({
        organizationId: data.organizationId,
        projectId: data.projectId,
      })
      .setSearchParams({
        userIds: data.ids,
      })
      .send();
  }
}

export const projectUserApiService = new ProjectUserApiService();
