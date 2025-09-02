import React from "react";
import { observer } from "mobx-react-lite";
import { Table, Avatar, Button } from "@chakra-ui/react";

import { UserModel } from "src/core-module/models/user.ts";
import { pickColor } from "src/shared-module/utils";

import { USERS_LIST_COLUMNS } from "./users-list.constants.ts";

type UsersListProps = {
  users: UserModel[];
  onUnassign: (user: UserModel) => void;
};

export const UsersList: React.FC<UsersListProps> = observer(({ users, onUnassign }) => {
  const handleUnassign = (user: UserModel) => {
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
              <Avatar.Root colorPalette={pickColor(user.fullName)}>
                <Avatar.Fallback name={user.fullName} />
              </Avatar.Root>
            </Table.Cell>

            {USERS_LIST_COLUMNS.map((col) => (
              <Table.Cell key={col.key}>{user[col.key as keyof UserModel]}</Table.Cell>
            ))}

            <Table.Cell>
              <Button colorPalette="red" onClick={() => handleUnassign(user)}>
                Unassign
              </Button>
            </Table.Cell>
          </Table.Row>
        ))}
      </Table.Body>
    </Table.Root>
  );
});
