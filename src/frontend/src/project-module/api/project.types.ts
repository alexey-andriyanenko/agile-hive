import type { ProjectModel } from "../models/project.ts";

export type GetProjectsResponse = ProjectModel[];

export type CreateProjectRequest = {
  name: string;
  description: string;
  organizationId: number;
};
export type CreateProjectResponse = ProjectModel;

export type UpdateProjectRequest = CreateProjectRequest & {
  projectId: number;
};

export type UpdateProjectResponse = ProjectModel;
