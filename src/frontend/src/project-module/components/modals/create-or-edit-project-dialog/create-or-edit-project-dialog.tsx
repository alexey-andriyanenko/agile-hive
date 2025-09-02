import React from "react";
import { useForm } from "react-hook-form";

import { Dialog, Button, Portal, Stack, Field, Input, Textarea } from "@chakra-ui/react";
import { ProjectFormValues } from "src/project-module/components/modals/create-or-edit-project-dialog/create-or-edit-project-dialog.types.ts";
import { ProjectModel } from "src/app-module/models/project.ts";
import { ModalProps } from "src/modals-module";

export type CreateOrEditProjectDialogProps = ModalProps & {
  onCreate?: (name: string, description: string) => Promise<void>;
  onEdit?: (name: string, description: string) => Promise<void>;

  project?: ProjectModel;
};

export const CreateOrEditProjectDialog: React.FC<CreateOrEditProjectDialogProps> = ({
  project,
  isOpen,
  onClose,
  onCreate,
  onEdit,
}) => {
  const { formState, register, handleSubmit } = useForm<ProjectFormValues>({
    defaultValues: {
      name: project?.name || "",
      description: project?.description || "",
    },
  });

  const onSubmit = handleSubmit(async (data) => {
    if (!project) {
      await onCreate?.(data.name, data.description);
    } else {
      await onEdit?.(data.name, data.description);
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
              <Dialog.Title>Create/Edit Project</Dialog.Title>
            </Dialog.Header>
            <Dialog.Body pb="4">
              <Stack gap="4">
                <Field.Root invalid={!!formState.errors.name}>
                  <Field.Label>Project Name</Field.Label>
                  <Input
                    {...register("name", {
                      required: {
                        value: true,
                        message: "Name is required",
                      },
                    })}
                  />
                  <Field.ErrorText>{formState.errors.name?.message}</Field.ErrorText>
                </Field.Root>

                <Field.Root invalid={!!formState.errors.description}>
                  <Field.Label>Project Description</Field.Label>
                  <Textarea
                    {...register("description", {
                      maxLength: {
                        value: 250,
                        message: "Description cannot exceed 250 characters",
                      },
                    })}
                  />
                  <Field.ErrorText>{formState.errors.description?.message}</Field.ErrorText>
                </Field.Root>
              </Stack>
            </Dialog.Body>
            <Dialog.Footer>
              <Dialog.ActionTrigger asChild>
                <Button variant="outline" onClick={onClose}>
                  Cancel
                </Button>
              </Dialog.ActionTrigger>
              <Button loading={formState.isSubmitting} onClick={onSubmit}>
                Save
              </Button>
            </Dialog.Footer>
          </Dialog.Content>
        </Dialog.Positioner>
      </Portal>
    </Dialog.Root>
  );
};
