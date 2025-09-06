import React from "react";
import { BrowserRouter, Route, Routes } from "react-router";

import { ThemeProvider } from "src/shared-module/components/theme";

import authModule from "src/auth-module";
import projectModule from "src/project-module";
import organizationModule from "src/organization-module";

import Index from "./pages/index";
import type { RouteItem } from "src/routes-module/routes-list/routes-list.types.ts";
import { PrivateRoute } from "src/routes-module/private-route";
import { PublicRoute } from "src/routes-module/public-route";
import NotFound from "src/app-module/pages/not-found";

const App: React.FC = () => {
  const renderRoutes = (routes: RouteItem[]) =>
    routes.map((props) =>
      props.isPrivate ? (
        <Route
          key={props.path}
          path={props.path}
          element={<PrivateRoute>{props.element}</PrivateRoute>}
        />
      ) : (
        <Route
          key={props.path}
          path={props.path}
          element={<PublicRoute>{props.element}</PublicRoute>}
        />
      ),
    );

  return (
    <ThemeProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Index />} />

          {renderRoutes(authModule)}
          {renderRoutes(organizationModule)}
          {renderRoutes(projectModule)}

          <Route
            path="*"
            element={
              <PublicRoute>
                <NotFound />
              </PublicRoute>
            }
          />
        </Routes>
      </BrowserRouter>
    </ThemeProvider>
  );
};

export default App;
