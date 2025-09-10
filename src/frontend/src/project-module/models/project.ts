import type { ProjectMemberRole } from "src/project-module/models/project-member-role.ts";

export type ProjectModel = {
  id: string;
  name: string;
  description: string;
  organizationId: string;
  slug: string;
  myRole: ProjectMemberRole;
};
