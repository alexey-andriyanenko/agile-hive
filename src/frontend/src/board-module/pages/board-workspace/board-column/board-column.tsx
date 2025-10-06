import React from "react";
import { observer } from "mobx-react-lite";
import { Flex, Heading, IconButton, Stack } from "@chakra-ui/react";
import { LuPlus } from "react-icons/lu";

import {
  useBoardStore,
  useModalsStore as useBoardModalsStore,
  useTaskStore,
} from "src/board-module/store";
import type { BoardColumnModel, TaskModel } from "src/board-module/models";
import { useOrganizationStore } from "src/organization-module/store";
import { useModalsStore as useSharedModalsStore } from "src/shared-module/store/modals";
import { taskStore } from "src/board-module/store/task.store.ts";
import { TaskCard } from "src/board-module/pages/board-workspace/board-column/task-card";

type BoardColumnProps = {
  column: BoardColumnModel;
};

export const BoardColumn: React.FC<BoardColumnProps> = observer(({ column }) => {
  const organizationStore = useOrganizationStore();
  const boardStore = useBoardStore();
  const boardModalsStore = useBoardModalsStore();
  const tasksStore = useTaskStore();
  const sharedModalsStore = useSharedModalsStore();

  const handleCreateTask = () => {
    boardModalsStore.open("CreateOrEditTaskDialog", {
      board: boardStore.currentBoard!,
      boardColumn: column,
      onCreate: (data) =>
        tasksStore.createTask({
          organizationId: organizationStore.currentOrganization!.id,
          projectId: boardStore.currentBoard!.projectId,
          boardId: boardStore.currentBoard!.id,
          title: data.title,
          description: JSON.stringify(data.description),
          boardColumnId: data.boardColumnId[0],
          assigneeUserId: data.assigneeId[0],
          tagIds: data.tagIds,
        }),
    });
  };

  const handleEditTask = (task: TaskModel) => {
    boardModalsStore.open("CreateOrEditTaskDialog", {
      task,
      board: boardStore.currentBoard!,
      boardColumn: column,
      onEdit: (data) =>
        tasksStore.updateTask({
          taskId: task.id,
          organizationId: organizationStore.currentOrganization!.id,
          projectId: task.projectId,
          boardId: task.boardId,
          title: data.title,
          description: JSON.stringify(data.description),
          boardColumnId: data.boardColumnId[0],
          assigneeUserId: data.assigneeId[0],
          tagIds: data.tagIds,
        }),
    });
  };

  const handleDeleteTask = (task: TaskModel) => {
    sharedModalsStore.open("ConfirmModal", {
      title: "Delete Task",
      description: "Are you sure you want to delete this task? This action cannot be undone.",
      onConfirm: () =>
        tasksStore.deleteTask({
          taskId: task.id,
          organizationId: organizationStore.currentOrganization!.id,
          projectId: task.projectId,
          boardId: task.boardId,
        }),
    });
  };

  return (
    <Stack
      width="300px"
      minWidth="300px"
      height="100%"
      padding={4}
      borderWidth="1px"
      borderColor="gray.200"
    >
      <Flex justifyContent="space-between">
        <Heading>{column.name}</Heading>
        <IconButton variant="outline" onClick={handleCreateTask}>
          <LuPlus />
        </IconButton>
      </Flex>

      {taskStore.tasksByColumnId[column.id]?.map((task) => (
        <TaskCard key={task.id} task={task} onEdit={handleEditTask} onDelete={handleDeleteTask} />
      ))}
    </Stack>
  );
});
