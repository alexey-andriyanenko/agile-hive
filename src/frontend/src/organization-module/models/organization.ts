import type { OrganizationMemberRole } from "src/organization-module/models/organization-member-role.ts";

export type OrganizationModel = {
  id: number;
  name: string;
  slug: string;
  myRole: OrganizationMemberRole;
};
