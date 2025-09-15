import type { AppSidebarNavItemProps } from "src/shared-module/layout";

export const getNavItems = (slug: string): AppSidebarNavItemProps[] => [
  {
    href: "/organization-selection",
    name: "Change Organization",
  },
  {
    href: `/organization/${slug}`,
    name: "Home",
  },
  {
    href: `/organization/${slug}/projects`,
    name: "Organization Projects",
  },
  {
    href: `/organization/${slug}/settings`,
    name: "Organization Settings",
  },
];
