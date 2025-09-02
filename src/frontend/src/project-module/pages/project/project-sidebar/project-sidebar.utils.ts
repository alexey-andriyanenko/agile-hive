import type { AppSidebarNavItemProps } from "src/shared-module/layout";

export const getNavItems = (
  organizationName: string,
  projectName: string,
): AppSidebarNavItemProps[] => [
  {
    href: `/organization/${organizationName}`,
    name: `Go to ${organizationName}`,
  },
  {
    href: `/organization/${organizationName}/projects/${projectName}`,
    name: "Project Home",
  },
  {
    href: `/organization/${organizationName}/projects/${projectName}/boards`,
    name: "Project Boards",
  },
  {
    href: `/organization/${organizationName}/projects/${projectName}/team`,
    name: "Project Team",
  },
  {
    href: `/organization/${organizationName}/projects/${projectName}/settings`,
    name: "Project Settings",
  },
];
