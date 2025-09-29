import React from "react";
import { observer } from "mobx-react-lite";
import { Button, Flex, Stack, Text } from "@chakra-ui/react";
import { useNavigate } from "react-router-dom";

import { useProjectStore } from "src/project-module/store";
import { useOrganizationStore } from "src/organization-module/store";

const InvalidBoard: React.FC = observer(() => {
  const navigate = useNavigate();
  const organizationStore = useOrganizationStore();
  const projectStore = useProjectStore();

  const handleGoToBoards = () => {
    navigate(
      `/organization/${organizationStore.currentOrganization!.slug}/projects/${projectStore.currentProject!.slug}/boards`,
    );
  };

  return (
    <Flex flex="1" width="100%" justify="center" pt={10}>
      <Stack>
        <Text> Board not found. </Text>
        <Button onClick={handleGoToBoards}>Go to Boards</Button>
      </Stack>
    </Flex>
  );
});

export default InvalidBoard;
