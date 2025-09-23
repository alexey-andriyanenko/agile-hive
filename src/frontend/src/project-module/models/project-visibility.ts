export enum ProjectVisibility {
  Public = 0,
  Private,
}

export const ProjectVisibilityToNameMap: Record<ProjectVisibility, string> = {
  [ProjectVisibility.Public]: "Public",
  [ProjectVisibility.Private]: "Private",
};
