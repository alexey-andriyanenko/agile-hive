import type { RouteItem } from "src/routes-module/routes-list/routes-list.types.ts";

import InvalidProject from "src/project-module/pages/invalid-project";
import Project from "src/project-module/pages/project";

export const ProjectRoutes = {
  invalid: "/organization/:organizationName/invalid-project",
  home: "/organization/:organizationName/projects/:projectName",
};

const routes: RouteItem[] = [
  {
    path: ProjectRoutes.invalid,
    element: <InvalidProject />,
    isPrivate: true,
  },
  {
    path: ProjectRoutes.home,
    element: <Project />,
    isPrivate: true,
  },
];

export default routes;
