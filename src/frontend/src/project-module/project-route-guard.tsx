import React from "react";
import { observer } from "mobx-react-lite";
import { useNavigate, useParams } from "react-router-dom";
import { useOrganizationStore } from "src/organization-module/store";
import { useProjectStore } from "src/project-module/store";

const ProjectRouteGuard: React.FC<React.PropsWithChildren> = observer(({ children }) => {
  const navigate = useNavigate();
  const routeParams = useParams<{ projectSlug: string }>();
  const organizationStore = useOrganizationStore();
  const projectStore = useProjectStore();
  const [verifyingProject, setVerifyingProject] = React.useState(true);

  React.useEffect(() => {
    if (!routeParams.projectSlug) {
      return;
    }

    if (
      projectStore.currentProject &&
      projectStore.currentProject.slug === routeParams.projectSlug
    ) {
      setVerifyingProject(false);
      return;
    }

    projectStore
      .fetchCurrentProjectBySlug(organizationStore.currentOrganization!.id, routeParams.projectSlug)
      .then(() => setVerifyingProject(false))
      .catch((error) => {
        navigate(`/organization/${organizationStore.currentOrganization!.slug}/invalid-project`);
        console.error("Failed to fetch project by slug:", error);
      });
  }, [routeParams.projectSlug, projectStore, organizationStore, navigate]);

  return verifyingProject ? <div>Verifying project...</div> : children;
});

export default ProjectRouteGuard;
