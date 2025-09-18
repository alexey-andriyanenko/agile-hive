import type { OrganizationUserRole } from "./organization-user-role.ts";

export type OrganizationUserModel = {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  userName: string;
  role: OrganizationUserRole;
};
