import React from "react";

import { Flex, Heading, Tabs } from "@chakra-ui/react";
import { LuUsers, LuTags } from "react-icons/lu";

import { ProjectSidebar } from "src/project-module/components/project-sidebar";

import { ProjectUsers } from "./project-users";
import { ProjectForm } from "src/project-module/pages/project-settings/project-form";
import { ProjectTags } from "./project-tags";

const ProjectSettings: React.FC = () => {
  return (
    <Flex flex="1" direction="row" width="100%" height="100%">
      <ProjectSidebar />

      <Flex direction="column" width="100%" p={4} gap={4}>
        <Heading> Project Settings </Heading>

        <ProjectForm />

        <Tabs.Root defaultValue="users">
          <Tabs.List>
            <Tabs.Trigger value="users">
              <LuUsers />
              Users
            </Tabs.Trigger>
            <Tabs.Trigger value="tags">
              <LuTags />
              Tags
            </Tabs.Trigger>
          </Tabs.List>
          <Tabs.Content value="users">
            <ProjectUsers />
          </Tabs.Content>
          <Tabs.Content value="tags">
            <ProjectTags />
          </Tabs.Content>
        </Tabs.Root>
      </Flex>
    </Flex>
  );
};

export default ProjectSettings;
