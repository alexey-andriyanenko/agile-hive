export enum ProjectMemberRole {
  Owner,
  Admin,
  Contributor,
  Viewer,
}

export const ProjectMemberRoleToNameMap: Record<ProjectMemberRole, string> = {
  [ProjectMemberRole.Owner]: "Owner",
  [ProjectMemberRole.Admin]: "Admin",
  [ProjectMemberRole.Contributor]: "Contributor",
  [ProjectMemberRole.Viewer]: "Viewer",
};
