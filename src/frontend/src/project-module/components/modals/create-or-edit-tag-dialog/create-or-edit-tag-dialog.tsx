import React from "react";
import { useForm } from "react-hook-form";

import { Dialog, Button, Portal, Stack, Field, Input } from "@chakra-ui/react";
import { observer } from "mobx-react-lite";

import type { ModalsPropsBase } from "src/modals-module";
import type { TagModel } from "src/project-module/models";

import type { TagFormValues } from "./create-or-edit-tag-dialog.types.ts";

export type CreateOrEditTagDialogProps = ModalsPropsBase & {
  tag?: TagModel;
  onCreate?: (data: TagFormValues) => Promise<void>;
  onEdit?: (data: TagFormValues) => Promise<void>;
};

export const CreateOrEditTagDialog: React.FC<CreateOrEditTagDialogProps> = observer(
  ({ tag, isOpen, onClose, onCreate, onEdit }) => {
    const isEditMode = !!tag;

    const { formState, register, handleSubmit } = useForm<TagFormValues>({
      defaultValues: {
        name: tag?.name ?? "",
        color: tag?.color ?? "#3b82f6",
      },
    });

    const onSubmit = handleSubmit(async (data) => {
      if (isEditMode && onEdit) {
        await onEdit(data);
      } else if (!isEditMode && onCreate) {
        await onCreate(data);
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
                <Dialog.Title>{isEditMode ? "Edit Tag" : "Create Tag"}</Dialog.Title>
              </Dialog.Header>
              <Dialog.Body pb="4">
                <Stack gap="4">
                  <Field.Root invalid={!!formState.errors.name}>
                    <Field.Label>Tag Name</Field.Label>
                    <Input
                      {...register("name", {
                        required: {
                          value: true,
                          message: "Name is required",
                        },
                        minLength: {
                          value: 2,
                          message: "Tag name must be at least 2 characters long",
                        },
                        maxLength: {
                          value: 50,
                          message: "Tag name cannot exceed 50 characters",
                        },
                      })}
                    />
                    <Field.ErrorText>{formState.errors.name?.message}</Field.ErrorText>
                  </Field.Root>

                  <Field.Root invalid={!!formState.errors.color}>
                    <Field.Label>Color</Field.Label>
                    <Input
                      type="color"
                      {...register("color", {
                        required: {
                          value: true,
                          message: "Color is required",
                        },
                        pattern: {
                          value: /^#([A-Fa-f0-9]{3}|[A-Fa-f0-9]{6})$/,
                          message: "Color must be in #xxx or #xxxxxx format",
                        },
                      })}
                    />
                    <Field.ErrorText>{formState.errors.color?.message}</Field.ErrorText>
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
                  {isEditMode ? "Save" : "Create"}
                </Button>
              </Dialog.Footer>
            </Dialog.Content>
          </Dialog.Positioner>
        </Portal>
      </Dialog.Root>
    );
  },
);
