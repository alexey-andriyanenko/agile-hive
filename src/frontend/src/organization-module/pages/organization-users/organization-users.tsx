import React from "react";

import { Flex, Button } from "@chakra-ui/react";
import { observer } from "mobx-react-lite";

import { OrganizationSidebar } from "../organization/organization-sidebar";
import { UsersList } from "./users-list";
import { useModalsStore, useOrganizationStore } from "../../store";
import { UserModel } from "src/core-module/models/user";
import { useModalsStore as useSharedModalsStore } from "src/shared-module/store/modals";

const OrganizationUsers: React.FC = observer(() => {
  const organizationStore = useOrganizationStore();
  const modalsStore = useModalsStore();
  const sharedModalsStore = useSharedModalsStore();
  const [loading, setLoading] = React.useState(true);

  React.useEffect(() => {
    organizationStore
      .fetchUsersByOrganization()
      .then(() => setLoading(false))
      .catch((error) => {
        console.error("Failed to fetch users:", error);
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
    <Flex flex="1" direction="row" width="100%">
      <OrganizationSidebar />

      <Flex direction="column" width="100%" p={4}>
        {loading ? (
          <div>loading users...</div>
        ) : (
          <>
            <Flex justify="flex-end">
              <Button variant="outline" onClick={handleCreateUser}>
                Create User
              </Button>
            </Flex>

            <UsersList
              users={organizationStore.users}
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
