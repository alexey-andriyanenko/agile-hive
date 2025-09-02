import React from "react";
import { Routes } from "react-router-dom";
import { Route } from "react-router";

import { PublicRoute } from "src/routes-module/public-route";
import { PrivateRoute } from "src/routes-module/private-route";
import type { RouteItem } from ".//routes-list.types.ts";

type RoutesListProps = {
  routes: RouteItem[];
};

export const RoutesList: React.FC<RoutesListProps> = ({ routes }) => {
  return (
    <Routes>
      {routes.map((props) => (
        <>
          {props.isPrivate ? (
            <Route
              key={props.path}
              path={props.path}
              element={<PrivateRoute> {props.element} </PrivateRoute>}
            />
          ) : (
            <Route
              key={props.path}
              path={props.path}
              element={<PublicRoute>{props.element}</PublicRoute>}
            />
          )}
        </>
      ))}
    </Routes>
  );
};
