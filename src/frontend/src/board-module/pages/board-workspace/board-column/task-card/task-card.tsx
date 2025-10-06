import { Stack, Flex, Heading, Box, Text, Avatar, IconButton } from "@chakra-ui/react";
import React from "react";
import type { TaskModel } from "src/board-module/models";
import { pickColor } from "src/shared-module/utils";
import { TaskTag } from "src/board-module/components/task-tag";
import { LuPencil, LuTrash } from "react-icons/lu";

type TaskCardProps = {
  task: TaskModel;
  onEdit?: (task: TaskModel) => void;
  onDelete?: (task: TaskModel) => void;
};

export const TaskCard: React.FC<TaskCardProps> = ({ task, onEdit, onDelete }) => {
  const handleEdit = () => onEdit?.(task);

  const handleDelete = () => onDelete?.(task);

  return (
    <Stack gap="2" borderWidth="1px" p="4" borderRadius="md">
      <Heading size="sm">{task.title}</Heading>

      <Flex alignItems="center" gap="4">
        <Text>Assigned to:</Text>

        {task.assignedTo ? (
          <Avatar.Root colorPalette={pickColor(task.assignedTo.fullName)} size="sm">
            <Avatar.Fallback name={task.assignedTo.fullName} />
          </Avatar.Root>
        ) : (
          <Box ml="2" fontSize="sm" color="gray.500">
            Unassigned
          </Box>
        )}
      </Flex>

      {task.tags.length !== 0 ? (
        <Flex alignItems="center" gap="2" wrap="wrap">
          <Text>Tags:</Text>

          {task.tags.map((tag) => (
            <TaskTag key={tag.id} tag={tag} />
          ))}
        </Flex>
      ) : (
        <Box ml="2" fontSize="sm" color="gray.500">
          No tags
        </Box>
      )}

      <Flex justifyContent="flex-end" alignItems="center" gap="2">
        <IconButton variant="outline" onClick={handleEdit}>
          <LuPencil />
        </IconButton>

        <IconButton variant="outline" color="red" onClick={handleDelete}>
          <LuTrash />
        </IconButton>
      </Flex>
    </Stack>
  );
};
