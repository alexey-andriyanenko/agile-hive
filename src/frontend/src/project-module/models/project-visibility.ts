export enum ProjectVisibility {
  Public,
  Private,
}

export const ProjectVisibilityToNameMap: Record<ProjectVisibility, string> = {
  [ProjectVisibility.Public]: "Public",
  [ProjectVisibility.Private]: "Private",
};
