import type { ProjectUserRole } from "./project-user-role.ts";

export type ProjectUserModel = {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  userName: string;
  role: ProjectUserRole;
};
