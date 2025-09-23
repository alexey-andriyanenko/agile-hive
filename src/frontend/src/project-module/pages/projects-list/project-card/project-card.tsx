import React from "react";

import { Card, Avatar, Button } from "@chakra-ui/react";

import { pickColor } from "src/shared-module/utils";
import type { ProjectModel } from "src/project-module/models/project.ts";

type ProjectCardProps = {
  project: ProjectModel;
  onSelect: (project: ProjectModel) => void;
  onDelete: (project: ProjectModel) => void;
};

export const ProjectCard: React.FC<ProjectCardProps> = ({ project, onSelect, onDelete }) => {
  const handleSelect = () => onSelect(project);
  const handleDelete = () => onDelete(project);

  return (
    <Card.Root height="320px">
      <Card.Body gap="2">
        <Avatar.Root size="lg" colorPalette={pickColor(project.name)}>
          <Avatar.Fallback name="Project Name" />
        </Avatar.Root>
        <Card.Title mt="2">{project.name}</Card.Title>
        <Card.Description>{project.description}</Card.Description>
      </Card.Body>
      <Card.Footer justifyContent="flex-end">
        <Button colorPalette="red" onClick={handleDelete}>
          Delete
        </Button>
        <Button variant="outline" onClick={handleSelect}>
          Enter
        </Button>
      </Card.Footer>
    </Card.Root>
  );
};
