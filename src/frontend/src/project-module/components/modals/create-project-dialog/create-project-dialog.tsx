import React from "react";
import { Controller, useFieldArray, useForm, useWatch } from "react-hook-form";

import {
  Dialog,
  Button,
  Portal,
  Stack,
  Field,
  Input,
  Textarea,
  Flex,
  Select,
  Box,
  IconButton,
  createListCollection,
} from "@chakra-ui/react";
import { observer } from "mobx-react-lite";

import {
  ProjectUserRoleToNameMap,
  ProjectVisibility,
  ProjectVisibilityToNameMap,
} from "src/project-module/models";
import type { ModalsPropsBase } from "src/modals-module";
import { organizationUserApiService } from "src/organization-module/api";
import type { OrganizationUserModel } from "src/organization-module/models/organization-user.ts";
import type { OrganizationModel } from "src/organization-module/models/organization.ts";
import { toSlug } from "src/shared-module/utils/to-slug/to-slug.ts";
import { useAuthStore } from "src/auth-module/store";

import type { ProjectFormValues } from "./create-project-dialog.types.ts";

export type CreateProjectDialogProps = ModalsPropsBase & {
  organization: OrganizationModel;
  onCreate: (data: ProjectFormValues) => Promise<void>;
};

export const CreateProjectDialog: React.FC<CreateProjectDialogProps> = observer(
  ({ organization, isOpen, onClose, onCreate }) => {
    const authStore = useAuthStore();
    const { formState, control, register, watch, handleSubmit } = useForm<ProjectFormValues>({
      defaultValues: {
        name: "",
        description: "",
        users: [],
        visibility: [ProjectVisibility.Public.toString()],
      },
    });
    const [users, setUsers] = React.useState<OrganizationUserModel[]>([]);
    const [loadingUsers, setLoadingUsers] = React.useState(true);
    const { fields, append, remove } = useFieldArray({
      control,
      name: "users",
    });

    // use watch tracks property changes inside each user element in the array
    const watchedUsers = useWatch({
      control,
      name: "users",
    });
    const watchedName = watch("name");

    const [userOptions, setUserOptions] = React.useState(
      createListCollection<{ label: string; value: string }>({ items: [] }),
    );
    const [filteredUserOptions, setFilteredUserOptions] = React.useState(
      createListCollection<{ label: string; value: string }>({ items: [] }),
    );

    const roleOptions = React.useMemo(() => {
      return createListCollection({
        items: Object.entries(ProjectUserRoleToNameMap).map(([value, label]) => ({
          label,
          value,
        })),
      });
    }, []);

    React.useEffect(() => {
      organizationUserApiService
        .getManyOrganizationUsers({ organizationId: organization.id })
        .then((response) => {
          setUsers(response.users);
          setUserOptions(
            createListCollection({
              items: response.users
                .filter((user) => user.id !== authStore.currentUser!.id)
                .map((x) => ({
                  label: `${x.firstName} ${x.lastName}`,
                  value: x.id,
                })),
            }),
          );
          setLoadingUsers(false);
        });
    }, []);

    React.useEffect(() => {
      console.log("watchedUsers", watchedUsers);

      const addedFormUserIds = watchedUsers.map((user) => user.userId[0]).filter((id) => !!id);

      setFilteredUserOptions(
        createListCollection({
          items: userOptions.items.filter((item) => !addedFormUserIds.includes(item.value)),
        }),
      );
    }, [users, watchedUsers, userOptions, authStore.currentUser]);

    const onSubmit = handleSubmit(async (data) => {
      await onCreate(data);
      onClose();
    });

    const handleAppend = () => {
      append({ userId: [], role: [] });
    };

    const visibilityOptions = React.useMemo(() => {
      return createListCollection({
        items: Object.entries(ProjectVisibilityToNameMap).map(([value, label]) => ({
          label,
          value,
        })),
      });
    }, []);

    return (
      <Dialog.Root lazyMount placement="center" open={isOpen}>
        <Portal>
          <Dialog.Backdrop />
          <Dialog.Positioner>
            <Dialog.Content>
              <Dialog.Header>
                <Dialog.Title>Create project</Dialog.Title>
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
                        minLength: {
                          value: 2,
                          message: "Project name must be at least 2 characters long",
                        },
                        pattern: {
                          value: /^(?!.* {2,})[A-Za-z0-9 ]+$/,
                          message:
                            "Project name can only contain letters, numbers, and spaces, and cannot contain consecutive spaces",
                        },
                      })}
                    />
                    <Field.ErrorText>{formState.errors.name?.message}</Field.ErrorText>
                  </Field.Root>

                  <Field.Root>
                    <Field.Label>Project URL (auto-generated)</Field.Label>
                    <Input value={toSlug(watchedName)} readOnly disabled />
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

                  <Field.Root invalid={!!formState.errors.visibility}>
                    <Field.Label>Visibility</Field.Label>
                    <Controller
                      control={control}
                      rules={{ required: "User is required" }}
                      name="visibility"
                      render={({ field }) => (
                        <Select.Root
                          name={field.name}
                          value={field.value}
                          onValueChange={(item) => field.onChange(item.value)}
                          onInteractOutside={() => field.onBlur()}
                          collection={visibilityOptions}
                        >
                          <Select.HiddenSelect />
                          <Select.Control>
                            <Select.Trigger>
                              <Select.ValueText placeholder="Select user" />
                            </Select.Trigger>
                            <Select.IndicatorGroup>
                              <Select.Indicator />
                            </Select.IndicatorGroup>
                          </Select.Control>
                          <Select.Positioner>
                            <Select.Content>
                              {visibilityOptions.items.map((item) => (
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
                    <Field.ErrorText>{formState.errors.visibility?.message}</Field.ErrorText>
                  </Field.Root>

                  {loadingUsers ? (
                    <Box>Loading users...</Box>
                  ) : (
                    <Stack gap={4}>
                      {fields.map((field, index) => (
                        <Flex gap={2} alignItems="flex-start" key={field.id}>
                          <Field.Root invalid={!!formState.errors.users?.[index]?.userId}>
                            <Field.Label>User</Field.Label>
                            <Controller
                              control={control}
                              rules={{ required: "User is required" }}
                              name={`users.${index}.userId`}
                              render={({ field }) => (
                                <Select.Root
                                  name={field.name}
                                  value={field.value}
                                  onValueChange={(item) => field.onChange(item.value)}
                                  onInteractOutside={() => field.onBlur()}
                                  collection={userOptions}
                                >
                                  <Select.HiddenSelect />
                                  <Select.Control>
                                    <Select.Trigger>
                                      <Select.ValueText placeholder="Select user" />
                                    </Select.Trigger>
                                    <Select.IndicatorGroup>
                                      <Select.Indicator />
                                    </Select.IndicatorGroup>
                                  </Select.Control>
                                  <Select.Positioner>
                                    <Select.Content>
                                      {filteredUserOptions.items.map((item) => (
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
                            <Field.ErrorText>
                              {formState.errors.users?.[index]?.userId?.message}
                            </Field.ErrorText>
                          </Field.Root>

                          <Field.Root invalid={!!formState.errors.users?.[index]?.role}>
                            <Field.Label>Role</Field.Label>
                            <Controller
                              control={control}
                              name={`users.${index}.role`}
                              rules={{ required: "Role is required" }}
                              render={({ field }) => (
                                <Select.Root
                                  name={field.name}
                                  value={field.value}
                                  onValueChange={(item) => field.onChange(item.value)}
                                  onInteractOutside={() => field.onBlur()}
                                  collection={roleOptions}
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
                                      {roleOptions.items.map((item) => (
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
                            <Field.ErrorText>
                              {formState.errors.users?.[index]?.role?.message}
                            </Field.ErrorText>
                          </Field.Root>

                          {fields.length > 1 && (
                            <Box pt={5}>
                              <Box pt={1.5}>
                                <IconButton
                                  variant="outline"
                                  colorScheme="red"
                                  onClick={() => remove(index)}
                                >
                                  -
                                </IconButton>
                              </Box>
                            </Box>
                          )}
                        </Flex>
                      ))}

                      {filteredUserOptions.items.length > 0 ? (
                        <Button variant="ghost" onClick={handleAppend}>
                          + Add user
                        </Button>
                      ) : (
                        <Flex justify="center">No more users to add</Flex>
                      )}
                    </Stack>
                  )}
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
  },
);
