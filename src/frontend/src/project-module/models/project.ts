import type { ProjectUserRole } from "src/project-module/models/project-user-role.ts";

export type ProjectModel = {
  id: string;
  name: string;
  description: string;
  organizationId: string;
  slug: string;
  myRole: ProjectUserRole;
};
