import React from "react";

import { Flex, Button, Heading } from "@chakra-ui/react";
import { observer } from "mobx-react-lite";

import { UsersList } from "./users-list";
import { useModalsStore, useOrganizationStore, useOrganizationUserStore } from "../../../store";
import { useModalsStore as useSharedModalsStore } from "src/shared-module/store/modals";

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
    modalsStore.open("CreateOrEditUserDialog", {
      onCreate: (data) =>
        organizationStore.createUser({
          username: data.username,
          email: data.email,
          fullName: data.fullName,
          password: data.password,
          role: "USER",
          orgId: organizationStore.currentOrganization!.id,
        }),
    });
  };

  const handleEditUser = (user: UserModel) => {
    modalsStore.open("CreateOrEditUserDialog", {
      user,
      onEdit: (data) =>
        organizationStore.updateUser({
          id: user.id,
          username: data.username,
          email: data.email,
          fullName: data.fullName,
          role: user.role,
        }),
    });
  };

  const handleDeleteUser = (user: UserModel) => {
    sharedModalsStore.open("ConfirmModal", {
      title: "Are you sure you want to delete this user?",
      description: `This action cannot be undone. User: ${user.fullName}`,
      onConfirm: () => organizationStore.deleteUser(user.id),
    });
  };

  return (
    <Flex flex="1" direction="column" width="100%">
      <Heading> Organization Team </Heading>

      <Flex direction="column" width="100%" p={4}>
        {loading ? (
          <div>Loading users...</div>
        ) : (
          <>
            <Flex justify="flex-end">
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
