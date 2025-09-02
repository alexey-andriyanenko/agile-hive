import type { RouteItem } from "src/routes-module/routes-list/routes-list.types.ts";

import OrganizationSelection from "src/organization-module/pages/organization-selection";
import InvalidOrganization from "src/organization-module/pages/invalid-organization";
import Organization from "src/organization-module/pages/organization";
import OrganizationUsers from "src/organization-module/pages/organization-users";
import OrganizationSettings from "src/organization-module/pages/organization-settings";

export const OrganizationRoutes = {
  select: "/organization-selection",
  invalid: "/invalid-organization",
  home: "/organization/:organizationName",
  projects: "/organization/:organizationName/projects",
  users: "/organization/:organizationName/users",
  settings: "/organization/:organizationName/settings",
};

const routes: RouteItem[] = [
  {
    path: OrganizationRoutes.select,
    element: <OrganizationSelection />,
    isPrivate: true,
  },
  {
    path: OrganizationRoutes.invalid,
    element: <InvalidOrganization />,
    isPrivate: true,
  },
  {
    path: OrganizationRoutes.home,
    element: <Organization />,
    isPrivate: true,
  },
  {
    path: OrganizationRoutes.users,
    element: <OrganizationUsers />,
    isPrivate: true,
  },
  {
    path: OrganizationRoutes.settings,
    element: <OrganizationSettings />,
    isPrivate: true,
  },
];

export default routes;
