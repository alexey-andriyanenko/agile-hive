import React from "react";
import { observer } from "mobx-react-lite";
import { Flex, Box } from "@chakra-ui/react";
import { useNavigate } from "react-router-dom";

import { useModalsStore as useSharedModalsStore } from "src/shared-module/store/modals";
import { OrganizationSidebar } from "../organization/organization-sidebar";
import { ProjectCard } from "./project-card";
import { AddProjectCard } from "./add-project-card";
import { useOrganizationStore, useModalsStore, useProjectStore } from "../../store";
import { ProjectModel } from "../../models/project.ts";

const OrganizationProjects: React.FC = observer(() => {
  const navigate = useNavigate();
  const organizationStore = useOrganizationStore();
  const projectStore = useProjectStore();
  const projectModalsStore = useModalsStore();
  const sharedModalsStore = useSharedModalsStore();
  const [loading, setLoading] = React.useState(false);

  React.useEffect(() => {
    if (projectStore.projects.length > 0) {
      return;
    }

    setLoading(true);

    projectStore
      .fetchProjects(organizationStore.currentOrganization!.id)
      .then(() => {
        setLoading(false);
      })
      .catch((error) => {
        console.error("Failed to fetch projects:", error);
        setLoading(false);
      });
  }, []);

  const handleSelectProject = (project: ProjectModel) => {
    projectStore.setCurrentProject(project);
    navigate(
      `/organization/${organizationStore.currentOrganization!.name}/projects/${project.name}`,
    );
  };

  const handleCreateProject = () => {
    projectModalsStore.open("CreateOrEditProjectDialog", {
      onCreate: async (name: string, description: string) =>
        projectStore.createProject({
          name,
          description,
          organizationId: organizationStore.currentOrganization!.id,
        }),
    });
  };

  const handleEditProject = (project: ProjectModel) => {
    projectModalsStore.open("CreateOrEditProjectDialog", {
      project,
      onEdit: async (name: string, description: string) =>
        projectStore.updateProject({
          projectId: project.id,
          name,
          description,
          organizationId: organizationStore.currentOrganization!.id,
        }),
    });
  };

  const handleDeleteProject = (project: ProjectModel) => {
    sharedModalsStore.open("ConfirmModal", {
      title: "Are you sure you want to delete this project?",
      description: `This action cannot be undone. Project: ${project.name}`,
      onConfirm: async () => {
        await projectStore.deleteProject(organizationStore.currentOrganization!.id, project.id);

        if (projectStore.currentProject?.id === project.id) {
          navigate(`/organizations/${organizationStore.currentOrganization!.name}/projects`);
        }
      },
    });
  };

  return (
    <Flex flex="1" direction="row" width="100%">
      <OrganizationSidebar />

      <Box
        display="grid"
        gridTemplateColumns="1fr 1fr 1fr"
        gridTemplateRows="repeat(auto-fill, 320px)"
        width="100%"
        p={4}
        gap={4}
      >
        {loading ? (
          <div>loading projects...</div>
        ) : (
          <>
            {projectStore.projects.map((project) => (
              <ProjectCard
                key={project.id}
                project={project}
                onSelect={handleSelectProject}
                onEdit={handleEditProject}
                onDelete={handleDeleteProject}
              />
            ))}

            <AddProjectCard onClick={handleCreateProject} />
          </>
        )}
      </Box>
    </Flex>
  );
});

export default OrganizationProjects;
