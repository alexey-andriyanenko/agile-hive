import React from "react";
import { Controller, useFieldArray, useForm, useWatch } from "react-hook-form";
import {
  Dialog,
  Button,
  Portal,
  Stack,
  Field,
  createListCollection,
  Select,
  Flex,
  IconButton,
  Box,
} from "@chakra-ui/react";
import type { ModalsPropsBase } from "src/modals-module";

import { organizationUserApiService } from "src/organization-module/api";
import type { OrganizationUserModel } from "src/organization-module/models/organization-user.ts";
import {
  type ProjectUserModel,
  ProjectUserRole,
  ProjectUserRoleToNameMap,
} from "src/project-module/models";
import type { AddUserToProjectItem } from "src/project-module/api";

import type { AddUserToProjectFormValues } from "./add-users-to-project-dialog.types.ts";
import type { OrganizationModel } from "src/organization-module/models/organization.ts";

export type AddUsersToProjectDialogProps = ModalsPropsBase & {
  organization: OrganizationModel;
  addedUsers: ProjectUserModel[];
  onAssign: (data: AddUserToProjectItem[]) => Promise<void>;
};

export const AddUsersToProjectDialog: React.FC<AddUsersToProjectDialogProps> = ({
  organization,
  addedUsers,
  isOpen,
  onAssign,
  onClose,
}) => {
  const [loading, setLoading] = React.useState(true);
  const [users, setUsers] = React.useState<OrganizationUserModel[]>([]);
  const { formState, control, handleSubmit } = useForm<AddUserToProjectFormValues>({
    defaultValues: {
      users: [
        {
          userId: [],
          role: [],
        },
      ],
    },
  });
  const { fields, append, remove } = useFieldArray({
    control,
    name: "users",
  });
  const watchedUsers = useWatch({
    control,
    name: "users",
  });
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
            items: response.users.map((x) => ({
              label: `${x.firstName} ${x.lastName}`,
              value: x.id,
            })),
          }),
        );
        setLoading(false);
      });
  }, []);

  React.useEffect(() => {
    console.log("watchedUsers", watchedUsers);

    const addedFormUserIds = watchedUsers
      .map((user) => user.userId?.[0])
      .filter((id): id is string => !!id);
    const addedUserIds = addedUsers.map((user) => user.id);

    const ids = [...addedFormUserIds, ...addedUserIds];

    setFilteredUserOptions(
      createListCollection({
        items: userOptions.items.filter((item) => !ids.includes(item.value)),
      }),
    );
  }, [users, addedUsers, userOptions, watchedUsers]);

  const onSubmit = handleSubmit(async (data) => {
    const userItems = data.users.map((user) => ({
      userId: user.userId[0],
      role: Number(user.role[0]) as ProjectUserRole,
    }));

    await onAssign(userItems);

    onClose();
  });

  const handleAppend = () => {
    append({ userId: [], role: [] });
  };

  return (
    <Dialog.Root lazyMount placement="center" open={isOpen}>
      <Portal>
        <Dialog.Backdrop />
        <Dialog.Positioner>
          <Dialog.Content>
            <Dialog.Header>
              <Dialog.Title>Assign user to project</Dialog.Title>
            </Dialog.Header>
            <Dialog.Body pb="4">
              {loading ? (
                <div>Loading users...</div>
              ) : (
                <Stack gap="4">
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
                      + Add another user
                    </Button>
                  ) : (
                    <Flex justify="center">No more users to add</Flex>
                  )}
                </Stack>
              )}
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
