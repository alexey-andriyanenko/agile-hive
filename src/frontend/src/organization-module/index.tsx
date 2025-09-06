import type { RouteItem } from "src/routes-module/routes-list/routes-list.types.ts";

import OrganizationSelection from "src/organization-module/pages/organization-selection";
import InvalidOrganization from "src/organization-module/pages/invalid-organization";
import Organization from "src/organization-module/pages/organization";
import OrganizationUsers from "src/organization-module/pages/organization-users";
import OrganizationSettings from "src/organization-module/pages/organization-settings";
import { Providers as SharedProviders } from "src/shared-module/providers";
import { Providers as OrganizationProviders } from "src/organization-module/providers";
import OrganizationRouteGuard from "src/organization-module/organization-route-guard.tsx";

export const OrganizationRoutes = {
  select: "/organization-selection",
  invalid: "/invalid-organization",
  home: "/organization/:organizationSlug",
  projects: "/organization/:organizationSlug/projects",
  users: "/organization/:organizationSlug/users",
  settings: "/organization/:organizationSlug/settings",
};

const routes: RouteItem[] = [
  {
    path: OrganizationRoutes.select,
    element: (
      <SharedProviders>
        <OrganizationProviders>
          <OrganizationSelection />
        </OrganizationProviders>
      </SharedProviders>
    ),
    isPrivate: true,
  },
  {
    path: OrganizationRoutes.invalid,
    element: (
      <SharedProviders>
        <OrganizationProviders>
          <InvalidOrganization />
        </OrganizationProviders>
      </SharedProviders>
    ),
    isPrivate: true,
  },
  {
    path: OrganizationRoutes.home,
    element: (
      <OrganizationRouteGuard>
        <SharedProviders>
          <OrganizationProviders>
            <Organization />
          </OrganizationProviders>
        </SharedProviders>
      </OrganizationRouteGuard>
    ),
    isPrivate: true,
  },
  {
    path: OrganizationRoutes.users,
    element: (
      <OrganizationRouteGuard>
        <SharedProviders>
          <OrganizationProviders>
            <OrganizationUsers />
          </OrganizationProviders>
        </SharedProviders>
      </OrganizationRouteGuard>
    ),
    isPrivate: true,
  },
  {
    path: OrganizationRoutes.settings,
    element: (
      <OrganizationRouteGuard>
        <SharedProviders>
          <OrganizationProviders>
            <OrganizationSettings />
          </OrganizationProviders>
        </SharedProviders>
      </OrganizationRouteGuard>
    ),
    isPrivate: true,
  },
];

export default routes;
