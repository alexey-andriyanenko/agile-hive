import React from "react";
import { Controller, useForm } from "react-hook-form";

import {
  Button,
  createListCollection,
  Dialog,
  Field,
  Input,
  Portal,
  Select,
  Stack,
} from "@chakra-ui/react";
import type { ModalsPropsBase } from "src/modals-module";
import {
  OrganizationUserRole,
  OrganizationUserRoleToNameMap,
} from "src/organization-module/models/organization-user-role.ts";
import type { OrganizationUserModel } from "src/organization-module/models/organization-user.ts";

type OrganizationUserFormModel = {
  firstName: string;
  lastName: string;
  email: string;
  userName: string;
  password: string;
  confirmPassword: string;
  role: string[];
};

export type CreateOrEditOrganizationUserDialogProps = ModalsPropsBase & {
  user?: OrganizationUserModel;
  onCreate?: (data: OrganizationUserFormModel) => Promise<void>;
  onEdit?: (data: OrganizationUserFormModel) => Promise<void>;
};

export const CreateOrEditOrganizationUserDialog: React.FC<
  CreateOrEditOrganizationUserDialogProps
> = ({ user, isOpen, onClose, onCreate, onEdit }) => {
  const { formState, control, register, handleSubmit } = useForm<OrganizationUserFormModel>({
    defaultValues: {
      firstName: user?.firstName || "",
      lastName: user?.lastName || "",
      email: user?.email || "",
      userName: user?.userName || "",
      password: "",
      confirmPassword: "",
      role: user ? [user.role.toString()] : [OrganizationUserRole.Member.toString()],
    },
  });

  const collection = React.useMemo(
    () =>
      createListCollection({
        items: Object.entries(OrganizationUserRoleToNameMap).map(([value, label]) => ({
          label,
          value,
        })),
      }),
    [],
  );

  const onSubmit = handleSubmit(async (data) => {
    if (user) {
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
                <Field.Root invalid={!!formState.errors.firstName}>
                  <Field.Label>First Name</Field.Label>
                  <Input
                    {...register("firstName", {
                      required: {
                        value: true,
                        message: "First name is required",
                      },
                      maxLength: {
                        value: 200,
                        message: "First name cannot exceed 200 characters",
                      },
                    })}
                  />
                  <Field.ErrorText>{formState.errors.firstName?.message}</Field.ErrorText>
                </Field.Root>

                <Field.Root invalid={!!formState.errors.lastName}>
                  <Field.Label>Last Name</Field.Label>
                  <Input
                    {...register("lastName", {
                      required: {
                        value: true,
                        message: "Last name is required",
                      },
                      maxLength: {
                        value: 200,
                        message: "Last name cannot exceed 200 characters",
                      },
                    })}
                  />
                  <Field.ErrorText>{formState.errors.lastName?.message}</Field.ErrorText>
                </Field.Root>

                <Field.Root invalid={!!formState.errors.email}>
                  <Field.Label>Email</Field.Label>
                  <Input
                    {...register("email", {
                      required: {
                        value: true,
                        message: "Email is required",
                      },
                      pattern: {
                        value: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/,
                        message: "Invalid email address",
                      },
                    })}
                  />
                  <Field.ErrorText>{formState.errors.email?.message}</Field.ErrorText>
                </Field.Root>

                <Field.Root invalid={!!formState.errors.userName}>
                  <Field.Label>Username</Field.Label>
                  <Input
                    {...register("userName", {
                      required: {
                        value: true,
                        message: "Username is required",
                      },
                      maxLength: {
                        value: 100,
                        message: "Username cannot exceed 100 characters",
                      },
                      pattern: {
                        value: /^(?!\d)[A-Za-z0-9_]+$/,
                        message:
                          "Username can contain only letters, numbers, and underscores, and cannot start with a number",
                      },
                    })}
                  />
                  <Field.ErrorText>{formState.errors.userName?.message}</Field.ErrorText>
                </Field.Root>

                <Field.Root invalid={!!formState.errors.password}>
                  <Field.Label>Password</Field.Label>
                  <Input
                    {...register("password", {
                      required: {
                        value: !user,
                        message: "Password is required",
                      },
                      pattern: {
                        value: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$/,
                        message:
                          "Password must contain at least 8 characters, include 1 special character, 1 digit, 1 lowercase and 1 uppercase character.",
                      },
                    })}
                  />
                  <Field.ErrorText>{formState.errors.password?.message}</Field.ErrorText>
                </Field.Root>

                <Field.Root invalid={!!formState.errors.confirmPassword}>
                  <Field.Label>Confirm Password</Field.Label>
                  <Input
                    {...register("confirmPassword", {
                      required: {
                        value: !user,
                        message: "Confirm password is required",
                      },
                      validate: (value, formValues) =>
                        value === formValues.password || "Passwords do not match",
                    })}
                  />
                  <Field.ErrorText>{formState.errors.confirmPassword?.message}</Field.ErrorText>
                </Field.Root>

                <Field.Root invalid={!!formState.errors.role}>
                  <Field.Label>Role</Field.Label>
                  <Controller
                    control={control}
                    name="role"
                    render={({ field }) => (
                      <Select.Root
                        name={field.name}
                        value={field.value}
                        onValueChange={(item) => field.onChange(item.value)}
                        onInteractOutside={() => field.onBlur()}
                        collection={collection}
                      >
                        <Select.HiddenSelect />
                        <Select.Control>
                          <Select.Trigger>
                            <Select.ValueText placeholder="Select role" />
                          </Select.Trigger>
                          <Select.IndicatorGroup>
                            <Select.Indicator />
                          </Select.IndicatorGroup>
                        </Select.Control>
                        <Select.Positioner>
                          <Select.Content>
                            {collection.items.map((item) => (
                              <Select.Item key={item.value} item={item}>
                                {item.label}
                                <Select.ItemIndicator />
                              </Select.Item>
                            ))}
                          </Select.Content>
                        </Select.Positioner>
                      </Select.Root>
                    )}
                  />
                  <Field.ErrorText>{formState.errors.role?.message}</Field.ErrorText>
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
