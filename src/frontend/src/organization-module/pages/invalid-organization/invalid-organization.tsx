import React from "react";
import { Button, Flex, Stack, Text } from "@chakra-ui/react";
import { useNavigate } from "react-router-dom";
import { OrganizationRoutes } from "src/organization-module/index.tsx";

const InvalidOrganization: React.FC = () => {
  const navigate = useNavigate();

  const handleGoToOrganizations = () => navigate(OrganizationRoutes.select);

  return (
    <Flex flex="1" width="100%" justify="center" pt={10}>
      <Stack>
        <Text> Organization not found. </Text>
        <Button onClick={handleGoToOrganizations}>Go to Organizations</Button>
      </Stack>
    </Flex>
  );
};

export default InvalidOrganization;
