import React from "react";

import { Flex, Button, Heading } from "@chakra-ui/react";
import { observer } from "mobx-react-lite";

import { UsersList } from "./users-list";
import { useModalsStore, useOrganizationStore, useOrganizationUserStore } from "../../../store";
import { useModalsStore as useSharedModalsStore } from "src/shared-module/store/modals";
import type { OrganizationUserModel } from "src/organization-module/models/organization-user.ts";

const OrganizationUsers: React.FC = observer(() => {
  const organizationStore = useOrganizationStore();
  const organizationUserStore = useOrganizationUserStore();
  const modalsStore = useModalsStore();
  const sharedModalsStore = useSharedModalsStore();
  const [loading, setLoading] = React.useState(true);

  React.useEffect(() => {
    organizationUserStore
      .fetchManyUsers({ organizationId: organizationStore.currentOrganization!.id })
      .then(() => setLoading(false))
      .catch((error) => {
        console.error("Failed to fetch organization users:", error);
        setLoading(false);
      });
  }, []);

  const handleCreateUser = () => {
    modalsStore.open("CreateOrEditOrganizationUserDialog", {
      onCreate: (data) =>
        organizationUserStore.createUser({
          organizationId: organizationStore.currentOrganization!.id,
          firstName: data.firstName,
          lastName: data.lastName,
          userName: data.userName,
          email: data.email,
          role: Number(data.role[0]),
          password: data.password,
        }),
    });
  };

  const handleEditUser = (user: OrganizationUserModel) => {
    modalsStore.open("CreateOrEditOrganizationUserDialog", {
      user,
      onEdit: (data) =>
        organizationUserStore.updateUser({
          id: user.id,
          organizationId: organizationStore.currentOrganization!.id,
          firstName: data.firstName,
          lastName: data.lastName,
          email: data.email,
          userName: data.userName,
          role: Number(data.role[0]),
          password: data.password,
        }),
    });
  };

  const handleDeleteUser = (user: OrganizationUserModel) => {
    sharedModalsStore.open("ConfirmModal", {
      title: `Are you sure you want to remove this user from current organization (${organizationStore.currentOrganization?.name})?`,
      description: `User: ${user.firstName} ${user.lastName} (${user.email})`,
      onConfirm: () =>
        organizationUserStore.removeUser({
          id: user.id,
          organizationId: organizationStore.currentOrganization!.id,
        }),
    });
  };

  return (
    <Flex flex="1" direction="column" width="100%" gap={4}>
      <Heading> Organization Team </Heading>

      <Flex direction="column" width="100%" p={4} gap={4}>
        {loading ? (
          <div>Loading users...</div>
        ) : (
          <>
            <Flex>
              <Button variant="outline" onClick={handleCreateUser}>
                Create User
              </Button>
            </Flex>

            <UsersList
              users={organizationUserStore.users}
              onEdit={handleEditUser}
              onDelete={handleDeleteUser}
            />
          </>
        )}
      </Flex>
    </Flex>
  );
});

export default OrganizationUsers;
