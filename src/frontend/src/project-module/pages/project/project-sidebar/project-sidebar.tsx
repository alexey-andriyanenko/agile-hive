import React from "react";
import { observer } from "mobx-react-lite";
//
// import { AppSidebar, AppSidebarNavItemProps } from "src/shared-module/layout";
// import { useOrganizationStore, useProjectStore } from "../../../store";
// import { getNavItems } from "./project-sidebar.utils.ts";

export const ProjectSidebar: React.FC = observer(() => {
  // const projectStore = useProjectStore();
  // const organizationStore = useOrganizationStore();
  //
  // const [navItems, setNavItems] = React.useState<AppSidebarNavItemProps[]>([]);
  //
  // React.useEffect(() => {
  //   setNavItems(
  //     getNavItems(organizationStore.currentOrganization!.name, projectStore.currentProject!.name),
  //   );
  // }, [organizationStore.currentOrganization, projectStore.currentProject]);
  //
  // return <AppSidebar title={projectStore.currentProject!.name} navItems={navItems} />;

  return <div>project-sidebar</div>;
});
