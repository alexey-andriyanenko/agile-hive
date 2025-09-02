import { appHttpClient } from "src/shared-module/api";
import type {
  CreateProjectRequest,
  CreateProjectResponse,
  GetProjectsResponse,
} from "./project.types.ts";

class ProjectApiService {
  public async getProjects(organizationId: number) {
    return await appHttpClient
      .get<GetProjectsResponse>("/api/core/organizations/:organizationId/projects")
      .setRouteParams({ organizationId })
      .send();
  }

  public async createProject(data: CreateProjectRequest) {
    return await appHttpClient
      .post<
        CreateProjectRequest,
        CreateProjectResponse
      >("/api/core/organizations/:organizationId/projects")
      .setRouteParams({ organizationId: data.organizationId })
      .send(data);
  }

  public async updateProject(data: CreateProjectRequest & { projectId: number }) {
    return await appHttpClient
      .put<
        CreateProjectRequest & { projectId: number },
        CreateProjectResponse
      >("/api/core/organizations/:organizationId/projects/:projectId")
      .setRouteParams({ organizationId: data.organizationId, projectId: data.projectId })
      .send(data);
  }

  public async deleteProject(organizationId: number, projectId: number) {
    return await appHttpClient
      .delete<void>("/api/core/organizations/:organizationId/projects/:projectId")
      .setRouteParams({ organizationId, projectId })
      .send();
  }
}

export const projectApiService = new ProjectApiService();
