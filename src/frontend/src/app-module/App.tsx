import React from "react";
import { BrowserRouter } from "react-router";

import { RoutesList } from "src/routes-module/routes-list/routes-list.tsx";
import { ThemeProvider } from "src/shared-module/components/theme";

import authModule from "src/auth-module";
import projectModule from "src/project-module";
import organizationModule from "src/organization-module";

const App: React.FC = () => {
  return (
    <ThemeProvider>
      <BrowserRouter>
        <RoutesList routes={authModule} />
        <RoutesList routes={projectModule} />
        <RoutesList routes={organizationModule} />
      </BrowserRouter>
    </ThemeProvider>
  );
};

export default App;
