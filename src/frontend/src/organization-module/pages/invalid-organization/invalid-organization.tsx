import React from "react";
import { Flex } from "@chakra-ui/react";

const InvalidOrganization: React.FC = () => {
  return (
    <Flex flex="1" justify="center" align="center" height="100%">
      Organization not found. Please check the URL or contact support.
    </Flex>
  );
};

export default InvalidOrganization;
