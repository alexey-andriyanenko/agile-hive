import React from "react";
import { observer } from "mobx-react-lite";

import { AppSidebar, type AppSidebarNavItemProps } from "src/shared-module/layout";
import { getNavItems } from "./organization-sidebar.utils.ts";
import { useOrganizationStore } from "src/organization-module/store";

export const OrganizationSidebar: React.FC = observer(() => {
  const organizationStore = useOrganizationStore();
  const [navItems, setNavItems] = React.useState<AppSidebarNavItemProps[]>([]);

  React.useEffect(() => {
    if (organizationStore.currentOrganization !== null) {
      setNavItems(getNavItems(organizationStore.currentOrganization.slug));
    }
  }, [organizationStore.currentOrganization]);

  return <AppSidebar title={organizationStore.currentOrganization!.name} navItems={navItems} />;
});
