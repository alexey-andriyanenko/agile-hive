import React from "react";

import { Flex } from "@chakra-ui/react";

import { OrganizationSidebar } from "../organization/organization-sidebar";

const OrganizationSettings: React.FC = () => {
  return (
    <Flex flex="1" direction="row" width="100%" height="100%">
      <OrganizationSidebar />

      <Flex direction="column" width="100%" p={4}>
        <h1>Organization settings</h1>
      </Flex>
    </Flex>
  );
};

export default OrganizationSettings;
