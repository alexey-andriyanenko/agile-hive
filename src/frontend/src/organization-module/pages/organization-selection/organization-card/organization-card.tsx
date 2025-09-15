import React from "react";

import { Card, Avatar, Button } from "@chakra-ui/react";

import { pickColor } from "src/shared-module/utils";
import type { OrganizationModel } from "../../../models/organization.ts";
import { OrganizationUserRoleToNameMap } from "src/organization-module/models/organization-user-role.ts";

type OrganizationCardProps = {
  organization: OrganizationModel;
  onSelect: (organization: OrganizationModel) => void;
};

export const OrganizationCard: React.FC<OrganizationCardProps> = ({ organization, onSelect }) => {
  const handleSelect = () => onSelect(organization);

  return (
    <Card.Root height="275px">
      <Card.Body gap="2">
        <Avatar.Root size="lg" colorPalette={pickColor(organization.name)}>
          <Avatar.Fallback name="organization name" />
        </Avatar.Root>
        <Card.Title mt="2">{organization.name}</Card.Title>
        <Card.Description>{OrganizationUserRoleToNameMap[organization.myRole]}</Card.Description>
      </Card.Body>
      <Card.Footer justifyContent="flex-end">
        <Button variant="outline" onClick={handleSelect}>
          Enter
        </Button>
      </Card.Footer>
    </Card.Root>
  );
};
