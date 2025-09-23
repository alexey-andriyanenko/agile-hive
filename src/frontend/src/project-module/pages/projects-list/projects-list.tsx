import React from "react";
import { observer } from "mobx-react-lite";
import { Flex, Box } from "@chakra-ui/react";
import { useNavigate } from "react-router-dom";

import { useModalsStore as useSharedModalsStore } from "src/shared-module/store/modals";

import { useModalsStore, useProjectStore } from "src/project-module/store";
import { useOrganizationStore } from "src/organization-module/store";
import type { ProjectModel } from "src/project-module/models/project.ts";
import { ProjectSidebar } from "src/project-module/components/project-sidebar";

import { ProjectCard } from "./project-card";
import { AddProjectCard } from "./add-project-card";

const ProjectsList: React.FC = observer(() => {
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
      .fetchManyProjects(organizationStore.currentOrganization!.id)
      .then(() => {
        setLoading(false);
      })
      .catch((error) => {
        console.error("Failed to fetch projects:", error);
        setLoading(false);
      });
  }, [organizationStore.currentOrganization, projectStore]);

  const handleSelectProject = (project: ProjectModel) => {
    projectStore.setCurrentProject(project);
    navigate(
      `/organization/${organizationStore.currentOrganization!.slug}/projects/${project.slug}`,
    );
  };

  const handleCreateProject = () => {
    projectModalsStore.open("CreateProjectDialog", {
      organization: organizationStore.currentOrganization!,
      onCreate: async (data) =>
        projectStore.createProject({
          name: data.name,
          description: data.description,
          users: data.users.map((user) => ({
            userId: user.userId[0],
            role: Number(user.role),
          })),
          visibility: Number(data.visibility),
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
          navigate(`/organizations/${organizationStore.currentOrganization!.slug}/projects`);
        }
      },
    });
  };

  return (
    <Flex flex="1" direction="row" width="100%">
      <ProjectSidebar />

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

export default ProjectsList;
