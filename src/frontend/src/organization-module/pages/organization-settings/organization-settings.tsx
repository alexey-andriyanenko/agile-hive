import React from "react";

import { Flex, Heading } from "@chakra-ui/react";

import { OrganizationSidebar } from "src/organization-module/components/organization-sidebar";
import OrganizationUsers from "./organization-users";
import { OrganizationForm } from "src/organization-module/pages/organization-settings/organization-form";

const OrganizationSettings: React.FC = () => {
  return (
    <Flex flex="1" direction="row" width="100%" height="100%">
      <OrganizationSidebar />

      <Flex direction="column" width="100%" p={4} gap={4}>
        <Heading> Organization Settings </Heading>

        <OrganizationForm />
        <OrganizationUsers />
      </Flex>
    </Flex>
  );
};

export default OrganizationSettings;
