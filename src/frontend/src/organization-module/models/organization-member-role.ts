export enum OrganizationMemberRole {
  Owner = 0, // Full control over tenant (billing, user mgmt, org settings)
  Admin, // Manage users and settings, but not billing/ownership transfer
  Manager, // Can create projects, assign project owners/admins
  Member, // Default user; can join projects when invited
  Guest, // Restricted access; can only be added to specific projects
}

export const OrganizationMemberRoleToNameMap: { [key in OrganizationMemberRole]: string } = {
  [OrganizationMemberRole.Owner]: "Owner",
  [OrganizationMemberRole.Admin]: "Admin",
  [OrganizationMemberRole.Manager]: "Manager",
  [OrganizationMemberRole.Member]: "Member",
  [OrganizationMemberRole.Guest]: "Guest",
};
