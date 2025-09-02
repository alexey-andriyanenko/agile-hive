import React from "react";
import { useForm } from "react-hook-form";

import { Dialog, Button, Portal, Stack, Field, Input, Textarea } from "@chakra-ui/react";
import { ProjectModel } from "../../models/project.ts";
import { ModalProps } from "src/modals-module";
import { OrganizationModel } from "../../models/organization.ts";

type OrganizationFormValues = {
  name: string;
  description: string;
};

export type CreateOrEditOrganizationDialogProps = ModalProps & {
  organization?: OrganizationModel;

  onCreate?: (data: OrganizationFormValues) => Promise<void>;
  onEdit?: (data: OrganizationFormValues) => Promise<void>;
};

export const CreateOrEditOrganizationDialog: React.FC<CreateOrEditOrganizationDialogProps> = ({
  organization,
  isOpen,
  onClose,
  onCreate,
  onEdit,
}) => {
  const { formState, register, handleSubmit } = useForm<OrganizationFormValues>({
    defaultValues: {
      name: organization?.name || "",
      description: organization?.description || "",
    },
  });

  const onSubmit = handleSubmit(async (data) => {
    if (organization) {
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
              <Dialog.Title>Create/Edit Organization</Dialog.Title>
            </Dialog.Header>
            <Dialog.Body pb="4">
              <Stack gap="4">
                <Field.Root invalid={!!formState.errors.name}>
                  <Field.Label>Organization Name</Field.Label>
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
                  <Field.Label>Organization Description</Field.Label>
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
