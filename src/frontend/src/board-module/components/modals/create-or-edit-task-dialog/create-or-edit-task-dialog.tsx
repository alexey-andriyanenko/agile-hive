import React from "react";
import { observer } from "mobx-react-lite";
import { FormProvider, useForm } from "react-hook-form";
import { Button, Dialog, Portal, Tabs } from "@chakra-ui/react";
import { LuMessageCircle, LuSettings } from "react-icons/lu";

import type { ModalsPropsBase } from "src/modals-module";
import type { BoardColumnModel, BoardModel, TaskModel } from "src/board-module/models";
import type { TaskFormValues } from "./create-or-edit-task-dialog.types.ts";
import { TaskDetailsForm } from "./task-details-form";
import { TaskCommentsForm } from "./task-comments-form";

export type CreateOrEditTaskDialogProps = ModalsPropsBase & {
  board: BoardModel;
  boardColumn: BoardColumnModel;
  task?: TaskModel;
  onCreate?: (data: TaskFormValues) => Promise<void>;
  onEdit?: (data: TaskFormValues) => Promise<void>;
};

export const CreateOrEditTaskDialog: React.FC<CreateOrEditTaskDialogProps> = observer(
  ({ isOpen, onClose, board, boardColumn, task, onEdit, onCreate }) => {
    const methods = useForm<TaskFormValues>({
      defaultValues: {
        title: task?.title || "",
        description: task ? JSON.parse(task.description) : {},
        boardColumnId: task ? [task.boardColumn.id] : [boardColumn.id],
        assigneeId: task?.assignedTo ? [task.assignedTo.id] : [],
        tagIds: task ? task.tags.map((tag) => tag.id) : [],
      },
    });

    const onSubmit = methods.handleSubmit(async (data) => {
      if (task) {
        await onEdit?.(data);
      } else {
        await onCreate?.(data);
      }

      onClose();
    });

    return (
      <Dialog.Root lazyMount placement="center" open={isOpen}>
        <Portal>
          <Dialog.Backdrop />
          <Dialog.Positioner>
            <Dialog.Content>
              <Dialog.Header>
                <Dialog.Title>{task ? "Edit task" : "Create task"}</Dialog.Title>
              </Dialog.Header>
              <Dialog.Body pb="4">
                <Tabs.Root defaultValue="details">
                  <Tabs.List>
                    <Tabs.Trigger value="details">
                      <LuSettings />
                      Details
                    </Tabs.Trigger>
                    <Tabs.Trigger value="comments">
                      <LuMessageCircle />
                      Comments
                    </Tabs.Trigger>
                  </Tabs.List>
                  <Tabs.Content value="details">
                    <FormProvider {...methods}>
                      <TaskDetailsForm board={board} />
                    </FormProvider>
                  </Tabs.Content>
                  <Tabs.Content value="comments">
                    <FormProvider {...methods}>
                      <TaskCommentsForm />
                    </FormProvider>
                  </Tabs.Content>
                </Tabs.Root>
              </Dialog.Body>
              <Dialog.Footer>
                <Dialog.ActionTrigger asChild>
                  <Button variant="outline" onClick={onClose}>
                    Cancel
                  </Button>
                </Dialog.ActionTrigger>
                <Button loading={methods.formState.isSubmitting} onClick={onSubmit}>
                  Save
                </Button>
              </Dialog.Footer>
            </Dialog.Content>
          </Dialog.Positioner>
        </Portal>
      </Dialog.Root>
    );
  },
);
