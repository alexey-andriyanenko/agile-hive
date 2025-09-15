import type { OrganizationUserRole } from "src/organization-module/models/organization-user-role.ts";

export type OrganizationModel = {
  id: string;
  name: string;
  slug: string;
  myRole: OrganizationUserRole;
};
