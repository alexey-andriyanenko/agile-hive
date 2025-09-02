import React from "react";

import { Card } from "@chakra-ui/react";
import { HiOutlinePlus } from "react-icons/hi";

import { useColorModeValue } from "src/shared-module/components/theme";

type AddOrganizationCardProps = {
  onClick: () => void;
};

export const AddOrganizationCard: React.FC<AddOrganizationCardProps> = ({ onClick }) => {
  return (
    <Card.Root
      height="275px"
      cursor="pointer"
      _hover={{
        backgroundColor: useColorModeValue("gray.200", "gray.400"),
        _active: { backgroundColor: useColorModeValue("gray.400", "gray.800") },
      }}
      onClick={onClick}
    >
      <Card.Body display="flex" justifyContent="center" alignItems="center" gap="2">
        <HiOutlinePlus size="80px" />
      </Card.Body>
    </Card.Root>
  );
};
