import type { AppSidebarNavItemProps } from "src/shared-module/layout";

export const getNavItems = (
  organizationSlug: string,
  projectSlug?: string,
): AppSidebarNavItemProps[] => {
  const result: AppSidebarNavItemProps[] = [
    {
      href: `/organization/${organizationSlug}`,
      name: `Go to ${organizationSlug}`,
    },
  ];

  if (projectSlug) {
    result.push(
      ...[
        {
          href: `/organization/${organizationSlug}/projects/${projectSlug}`,
          name: "Project Home",
        },
        {
          href: `/organization/${organizationSlug}/projects/${projectSlug}/boards`,
          name: "Project Boards",
        },
        {
          href: `/organization/${organizationSlug}/projects/${projectSlug}/settings`,
          name: "Project Settings",
        },
      ],
    );
  }

  return result;
};
