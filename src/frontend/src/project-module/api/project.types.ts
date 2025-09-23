import type { ProjectModel } from "../models/project.ts";
import type { ProjectUserRole } from "src/project-module/models/project-user-role.ts";
import type { ProjectVisibility } from "src/project-module/models/project-visibility.ts";

export type GetProjectsResponse = {
  projects: ProjectModel[];
};

export type CreateProjectRequest = {
  name: string;
  description: string;
  organizationId: string;
  visibility: ProjectVisibility;
  users: CreateProjectUserItem[];
};

export type CreateProjectUserItem = {
  userId: string;
  role: ProjectUserRole;
};

export type CreateProjectResponse = ProjectModel;

export type UpdateProjectRequest = {
  projectId: string;
  organizationId: string;
  name: string;
  description: string;
  visibility: ProjectVisibility;
};

export type UpdateProjectResponse = ProjectModel;
