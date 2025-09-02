import type { RouteProps } from "react-router-dom";

export type RouteItem = {
  path: Required<RouteProps>["path"];
  element: RouteProps["element"];
  isPrivate?: boolean;
};
