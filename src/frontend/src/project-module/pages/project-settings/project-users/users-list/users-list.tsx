import React from "react";
import { observer } from "mobx-react-lite";
import { Table, Avatar, Button } from "@chakra-ui/react";

import { type ProjectUserModel, ProjectUserRoleToNameMap } from "src/project-module/models";
import { pickColor } from "src/shared-module/utils";
import { useAuthStore } from "src/auth-module/store";

import { USERS_LIST_COLUMNS } from "./users-list.constants.ts";

type UsersListProps = {
  users: ProjectUserModel[];
  onUnassign: (user: ProjectUserModel) => void;
};

export const UsersList: React.FC<UsersListProps> = observer(({ users, onUnassign }) => {
  const authStore = useAuthStore();

  const handleUnassign = (user: ProjectUserModel) => {
    onUnassign(user);
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
              <Avatar.Root colorPalette={pickColor(`${user.firstName} ${user.lastName}`)}>
                <Avatar.Fallback name={`${user.firstName} ${user.lastName}`} />
              </Avatar.Root>
            </Table.Cell>

            {USERS_LIST_COLUMNS.map((col) => (
              <Table.Cell key={col.key}>
                {col.key === "role"
                  ? ProjectUserRoleToNameMap[user[col.key]]
                  : user[col.key as keyof ProjectUserModel]}
              </Table.Cell>
            ))}

            <Table.Cell>
              {authStore.currentUser?.id !== user.id && (
                <Button colorPalette="red" onClick={() => handleUnassign(user)}>
                  Unassign
                </Button>
              )}
            </Table.Cell>
          </Table.Row>
        ))}
      </Table.Body>
    </Table.Root>
  );
});
