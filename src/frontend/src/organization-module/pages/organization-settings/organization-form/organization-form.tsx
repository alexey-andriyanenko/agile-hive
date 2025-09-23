import React from "react";
import { observer } from "mobx-react-lite";
import { Button, Field, Flex, Input, Stack } from "@chakra-ui/react";
import { useForm } from "react-hook-form";

import { useOrganizationStore } from "src/organization-module/store";

import type { OrganizationFormValues } from "./organization-form.types.ts";
import { toSlug } from "src/shared-module/utils/to-slug/to-slug.ts";
import { useNavigate } from "react-router-dom";

export const OrganizationForm: React.FC = observer(() => {
  const navigate = useNavigate();
  const organizationStore = useOrganizationStore();
  const { formState, register, reset, watch, handleSubmit } = useForm<OrganizationFormValues>({
    defaultValues: {
      name: organizationStore.currentOrganization!.name,
    },
  });
  const currentName = watch("name");

  const onSubmit = handleSubmit(async (data: OrganizationFormValues) => {
    await organizationStore.updateOrganization({
      id: organizationStore.currentOrganization!.id,
      organizationName: data.name,
    });

    reset(data);

    navigate(`/organization/${organizationStore.currentOrganization!.slug}/settings`);
  });

  const onCancel = () => reset();

  return (
    <Stack gap={4} width="50%">
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

      <Flex alignItems="center" gap={4}>
        <Button variant="outline" onClick={onCancel} disabled={!formState.isDirty}>
          Cancel
        </Button>
        <Button
          variant="solid"
          onClick={onSubmit}
          disabled={!formState.isDirty}
          loading={formState.isSubmitting}
        >
          Submit
        </Button>
      </Flex>
    </Stack>
  );
});
