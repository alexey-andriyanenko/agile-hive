import React from "react";

import { Flex } from "@chakra-ui/react";

import { OrganizationSidebar } from "./organization-sidebar";

const Organization: React.FC = () => {
  return (
    <Flex flex="1" direction="row" width="100%" height="100%">
      <OrganizationSidebar />

      <Flex direction="column" width="100%" p={4}>
        <h1>Welcome to the Home Page</h1>
        <p>This is the main page of our application.</p>
      </Flex>
    </Flex>
  );
};

export default Organization;
