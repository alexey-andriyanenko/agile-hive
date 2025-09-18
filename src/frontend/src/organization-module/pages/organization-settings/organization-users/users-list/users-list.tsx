import React from "react";
import { observer } from "mobx-react-lite";
import { Table, Avatar, Menu, IconButton, Portal } from "@chakra-ui/react";
import { HiDotsVertical } from "react-icons/hi";

import { pickColor } from "src/shared-module/utils";
import type { OrganizationUserModel } from "src/organization-module/models/organization-user.ts";

import { USERS_LIST_COLUMNS } from "./users-list.constants.ts";
import { OrganizationUserRoleToNameMap } from "src/organization-module/models/organization-user-role.ts";
import { useAuthStore } from "src/auth-module/store";

type UsersListProps = {
  users: OrganizationUserModel[];
  onEdit: (user: OrganizationUserModel) => void;
  onDelete: (user: OrganizationUserModel) => void;
};

export const UsersList: React.FC<UsersListProps> = observer(({ users, onEdit, onDelete }) => {
  const authStore = useAuthStore();

  const handleMenu = (user: OrganizationUserModel, value: string) => {
    if (value === "edit") {
      onEdit(user);
    }

    if (value === "delete") {
      onDelete(user);
    }
  };

  return (
    <Table.Root>
      <Table.Header>
        <Table.Row>
          <Table.ColumnHeader>Avatar</Table.ColumnHeader>

          {USERS_LIST_COLUMNS.map((col) => (
            <Table.ColumnHeader key={col.key}>{col.label}</Table.ColumnHeader>
          ))}

          <Table.ColumnHeader>Actions</Table.ColumnHeader>
        </Table.Row>
      </Table.Header>

      <Table.Body>
        {users.map((user) => (
          <Table.Row key={user.id}>
            <Table.Cell>
              <Avatar.Root colorPalette={pickColor(`${user.firstName} ${user.lastName}`)} size="sm">
                <Avatar.Fallback name={`${user.firstName} ${user.lastName}`} />
              </Avatar.Root>
            </Table.Cell>

            {USERS_LIST_COLUMNS.map((col) => (
              <Table.Cell key={col.key}>
                {col.key === "role"
                  ? OrganizationUserRoleToNameMap[user[col.key]]
                  : user[col.key as keyof OrganizationUserModel]}
              </Table.Cell>
            ))}

            <Table.Cell>
              <Menu.Root onSelect={({ value }) => handleMenu(user, value)}>
                <Menu.Trigger asChild>
                  <IconButton variant="outline">
                    <HiDotsVertical />
                  </IconButton>
                </Menu.Trigger>
                <Portal>
                  <Menu.Positioner>
                    <Menu.Content>
                      <Menu.Item value="edit">Edit</Menu.Item>

                      {authStore.currentUser!.id !== user.id ? (
                        <Menu.Item value="delete">Delete</Menu.Item>
                      ) : null}
                    </Menu.Content>
                  </Menu.Positioner>
                </Portal>
              </Menu.Root>
            </Table.Cell>
          </Table.Row>
        ))}
      </Table.Body>
    </Table.Root>
  );
});
