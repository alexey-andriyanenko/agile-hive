import { AppSidebarNavItemProps } from "src/shared-module/layout";

export const getNavItems = (name: string): AppSidebarNavItemProps[] => [
  {
    href: "/organization-selection",
    name: "Change Organization",
  },
  {
    href: `/organization/${name}`,
    name: "Home",
  },
  {
    href: `/organization/${name}/projects`,
    name: "Organization Projects",
  },
  {
    href: `/organization/${name}/users`,
    name: "Organization Users",
  },
  {
    href: `/organization/${name}/settings`,
    name: "Organization Settings",
  },
];
