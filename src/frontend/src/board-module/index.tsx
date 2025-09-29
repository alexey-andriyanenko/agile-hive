import type { RouteItem } from "src/routes-module/routes-list/routes-list.types.ts";
import Boards from "src/board-module/pages/boards";
import InvalidBoard from "src/board-module/pages/invalid-board";
import { Providers as SharedProviders } from "src/shared-module/providers";
import OrganizationRouteGuard from "src/organization-module/organization-route-guard.tsx";
import ProjectRouteGuard from "src/project-module/project-route-guard.tsx";
import BoardRouteGuard from "src/board-module/board-route-guard.tsx";

export const BoardRoutes = {
  invalid: "/organization/:organizationSlug/projects/:projectSlug/invalid-board",
  boards: "/organization/:organizationSlug/projects/:projectSlug/boards",
  board: "/organization/:organizationSlug/projects/:projectSlug/boards/:boardId",
};

const routes: RouteItem[] = [
  {
    path: BoardRoutes.invalid,
    element: (
      <OrganizationRouteGuard>
        <ProjectRouteGuard>
          <SharedProviders>
            <InvalidBoard />
          </SharedProviders>
        </ProjectRouteGuard>
      </OrganizationRouteGuard>
    ),
    isPrivate: true,
  },
  {
    path: BoardRoutes.boards,
    element: (
      <OrganizationRouteGuard>
        <ProjectRouteGuard>
          <SharedProviders>
            <Boards />
          </SharedProviders>
        </ProjectRouteGuard>
      </OrganizationRouteGuard>
    ),
    isPrivate: true,
  },
  {
    path: BoardRoutes.board,
    element: (
      <OrganizationRouteGuard>
        <ProjectRouteGuard>
          <BoardRouteGuard>
            <SharedProviders></SharedProviders>
          </BoardRouteGuard>
        </ProjectRouteGuard>
      </OrganizationRouteGuard>
    ),
    isPrivate: true,
  },
];

export default routes;
