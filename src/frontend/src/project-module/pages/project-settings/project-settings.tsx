import React from "react";

import { Flex, Heading } from "@chakra-ui/react";

import { ProjectSidebar } from "src/project-module/components/project-sidebar";

import { ProjectUsers } from "./project-users";

const ProjectSettings: React.FC = () => {
  return (
    <Flex flex="1" direction="row" width="100%" height="100%">
      <ProjectSidebar />

      <Flex direction="column" width="100%" p={4} gap={4}>
        <Heading> Project Settings </Heading>

        <ProjectUsers />
      </Flex>
    </Flex>
  );
};

export default ProjectSettings;
