import React from "react";
import { observer } from "mobx-react-lite";
import { useNavigate, useParams } from "react-router-dom";

import { useOrganizationStore } from "src/organization-module/store";
import { OrganizationRoutes } from "src/organization-module/index.tsx";

const OrganizationRouteGuard: React.FC<React.PropsWithChildren> = observer(({ children }) => {
  const navigate = useNavigate();
  const routeParams = useParams<{ organizationSlug: string }>();
  const organizationStore = useOrganizationStore();
  const [verifyingOrganization, setVerifiyingOrganization] = React.useState(true);

  React.useEffect(() => {
    if (!routeParams.organizationSlug) {
      return;
    }

    if (
      organizationStore.currentOrganization &&
      organizationStore.currentOrganization.slug === routeParams.organizationSlug
    ) {
      setVerifiyingOrganization(false);
      return;
    }

    organizationStore
      .fetchCurrentOrganizationBySlug(routeParams.organizationSlug)
      .then(() => setVerifiyingOrganization(false))
      .catch((error) => {
        navigate(OrganizationRoutes.invalid);
        console.error("Failed to fetch organization by slug:", error);
      });
  }, [routeParams.organizationSlug, organizationStore, navigate]);

  return verifyingOrganization ? <div>Verifying organization...</div> : children;
});

export default OrganizationRouteGuard;
