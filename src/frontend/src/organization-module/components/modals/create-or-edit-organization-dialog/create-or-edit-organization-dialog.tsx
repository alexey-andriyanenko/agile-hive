import React from "react";
import { useForm } from "react-hook-form";

import { Dialog, Button, Portal, Stack, Field, Input } from "@chakra-ui/react";
import type { ModalsPropsBase } from "src/modals-module";
import type { OrganizationModel } from "src/organization-module/models/organization.ts";

type OrganizationFormValues = {
  name: string;
};

export type CreateOrEditOrganizationDialogProps = ModalsPropsBase & {
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
              <Dialog.Title>
                {organization ? "Edit organization" : "Create organization"}
              </Dialog.Title>
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
