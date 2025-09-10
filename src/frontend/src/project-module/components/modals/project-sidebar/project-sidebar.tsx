import React from "react";
import { observer } from "mobx-react-lite";

import { AppSidebar, type AppSidebarNavItemProps } from "src/shared-module/layout";

import { getNavItems } from "./project-sidebar.utils";
import { useProjectStore } from "src/project-module/store";
import { useOrganizationStore } from "src/organization-module/store";

export const ProjectSidebar: React.FC = observer(() => {
  const projectStore = useProjectStore();
  const organizationStore = useOrganizationStore();

  const [navItems, setNavItems] = React.useState<AppSidebarNavItemProps[]>([]);

  const title = React.useMemo(() => {
    return projectStore.currentProject
      ? `${organizationStore.currentOrganization!.name} / ${projectStore.currentProject!.name}`
      : `${organizationStore.currentOrganization!.name} / No Project Selected`;
  }, [organizationStore.currentOrganization, projectStore.currentProject]);

  React.useEffect(() => {
    setNavItems(
      getNavItems(organizationStore.currentOrganization!.slug, projectStore.currentProject?.slug),
    );
  }, [organizationStore.currentOrganization, projectStore.currentProject]);

  return <AppSidebar title={title} navItems={navItems} />;
});
