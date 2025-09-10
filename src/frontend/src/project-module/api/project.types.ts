import type { ProjectModel } from "../models/project.ts";
import type { ProjectMemberRole } from "src/project-module/models/project-member-role.ts";
import type { ProjectVisibility } from "src/project-module/models/project-visibility.ts";

export type GetProjectsResponse = {
  projects: ProjectModel[];
};

export type CreateProjectRequest = {
  name: string;
  description: string;
  organizationId: number;
  visibility: ProjectVisibility;
  members: CreateProjectMemberItem[];
};

export type CreateProjectMemberItem = {
  userId: number;
  role: ProjectMemberRole;
};

export type CreateProjectResponse = ProjectModel;

export type UpdateProjectRequest = CreateProjectRequest & {
  projectId: number;
};

export type UpdateProjectResponse = ProjectModel;
