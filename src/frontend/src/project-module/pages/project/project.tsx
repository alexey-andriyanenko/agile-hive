import React from "react";
import { Flex } from "@chakra-ui/react";

import { ProjectSidebar } from "./project-sidebar";

const Project: React.FC = () => {
  return (
    <Flex direction="row" width="100%" height="100vh">
      <ProjectSidebar />

      <Flex direction="column" width="100%" p={4}>
        <h1>Welcome to the Project Page</h1>
      </Flex>
    </Flex>
  );
};

export default Project;
