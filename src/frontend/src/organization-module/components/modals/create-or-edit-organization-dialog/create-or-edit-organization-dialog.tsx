import React from "react";
import { useForm } from "react-hook-form";

import { Dialog, Button, Portal, Stack, Field, Input } from "@chakra-ui/react";
import type { ModalsPropsBase } from "src/modals-module";
import type { OrganizationModel } from "src/organization-module/models/organization.ts";
import { toSlug } from "src/shared-module/utils/to-slug/to-slug.ts";

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
  const { formState, register, watch, handleSubmit } = useForm<OrganizationFormValues>({
    defaultValues: {
      name: organization?.name || "",
    },
  });
  const currentName = watch("name");

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
                      minLength: {
                        value: 2,
                        message: "Organization name must be at least 2 characters long",
                      },
                      pattern: {
                        value: /^(?!.* {2,})[A-Za-z0-9 ]+$/,
                        message:
                          "Organization name can only contain letters, numbers, and spaces, and cannot contain consecutive spaces",
                      },
                    })}
                  />
                  <Field.ErrorText>{formState.errors.name?.message}</Field.ErrorText>
                </Field.Root>

                <Input value={toSlug(currentName)} readOnly disabled />
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
