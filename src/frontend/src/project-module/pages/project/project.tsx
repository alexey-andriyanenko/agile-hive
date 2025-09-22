import React from "react";
import { Flex } from "@chakra-ui/react";

import { ProjectSidebar } from "src/project-module/components/project-sidebar";
import { useProjectStore } from "src/project-module/store";

const Project: React.FC = () => {
  const projectStore = useProjectStore();

  return (
    <Flex direction="row" width="100%" height="100vh">
      <ProjectSidebar />

      <Flex direction="column" width="100%" p={4}>
        <h1>Welcome to the Project Page</h1>
        <p>Current Project: {projectStore.currentProject?.name}</p>
      </Flex>
    </Flex>
  );
};

export default Project;
