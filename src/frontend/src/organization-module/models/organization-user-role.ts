export enum OrganizationUserRole {
  Owner = 0, // Full control over tenant (billing, user mgmt, org settings)
  Admin, // Manage users and settings, but not billing/ownership transfer
  Manager, // Can create projects, assign project owners/admins
  Member, // Default user; can join projects when invited
  Guest, // Restricted access; can only be added to specific projects
}

export const OrganizationUserRoleToNameMap: { [key in OrganizationUserRole]: string } = {
  [OrganizationUserRole.Owner]: "Owner",
  [OrganizationUserRole.Admin]: "Admin",
  [OrganizationUserRole.Manager]: "Manager",
  [OrganizationUserRole.Member]: "Member",
  [OrganizationUserRole.Guest]: "Guest",
};
