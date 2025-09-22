export enum ProjectUserRole {
  Owner,
  Admin,
  Contributor,
  Viewer,
}

export const ProjectUserRoleToNameMap: Record<ProjectUserRole, string> = {
  [ProjectUserRole.Owner]: "Owner",
  [ProjectUserRole.Admin]: "Admin",
  [ProjectUserRole.Contributor]: "Contributor",
  [ProjectUserRole.Viewer]: "Viewer",
};
