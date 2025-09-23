import type { ProjectUserRole } from "src/project-module/models/project-user-role.ts";
import type { ProjectVisibility } from "src/project-module/models/project-visibility.ts";

export type ProjectModel = {
  id: string;
  name: string;
  description: string;
  organizationId: string;
  slug: string;
  visibility: ProjectVisibility;
  myRole: ProjectUserRole;
};
