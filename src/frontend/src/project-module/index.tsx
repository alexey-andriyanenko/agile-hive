import type { RouteItem } from "src/routes-module/routes-list/routes-list.types.ts";

import InvalidProject from "src/project-module/pages/invalid-project";
import Project from "src/project-module/pages/project";
import ProjectsList from "src/project-module/pages/projects-list";

import { Providers as SharedProviders } from "src/shared-module/providers";
import { Providers as ProjectProviders } from "src/project-module/providers";
import OrganizationRouteGuard from "src/organization-module/organization-route-guard.tsx";
import ProjectRouteGuard from "src/project-module/project-route-guard.tsx";

export const ProjectRoutes = {
  invalid: "/organization/:organizationSlug/invalid-project",
  home: "/organization/:organizationSlug/projects/:projectSlug",
  projects: "/organization/:organizationSlug/projects",
};

const routes: RouteItem[] = [
  {
    path: ProjectRoutes.invalid,
    element: (
      <OrganizationRouteGuard>
        <SharedProviders>
          <ProjectProviders>
            <InvalidProject />
          </ProjectProviders>
        </SharedProviders>
      </OrganizationRouteGuard>
    ),
    isPrivate: true,
  },
  {
    path: ProjectRoutes.projects,
    element: (
      <OrganizationRouteGuard>
        <SharedProviders>
          <ProjectProviders>
            <ProjectsList />
          </ProjectProviders>
        </SharedProviders>
      </OrganizationRouteGuard>
    ),
    isPrivate: true,
  },
  {
    path: ProjectRoutes.home,
    element: (
      <OrganizationRouteGuard>
        <ProjectRouteGuard>
          <SharedProviders>
            <ProjectProviders>
              <Project />
            </ProjectProviders>
          </SharedProviders>
        </ProjectRouteGuard>
      </OrganizationRouteGuard>
    ),
    isPrivate: true,
  },
];

export default routes;
