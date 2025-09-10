import React from "react";
import { observer } from "mobx-react-lite";

import { Flex, Button } from "@chakra-ui/react";

import { ProjectSidebar } from "src/project-module/components/modals/project-sidebar";
import { UsersList } from "./users-list";
import { useModalsStore } from "src/core-module/store";
import { useModalsStore as useSharedModalsStore } from "../../../shared-module/store/modals";
import { UserModel } from "src/core-module/models/user";

const ProjectUsers: React.FC = observer(() => {
  const organizationStore = useOrganizationStore();
  const projectStore = useProjectStore();
  const modalsStore = useModalsStore();
  const sharedModalsStore = useSharedModalsStore();

  React.useEffect(() => {
    projectStore
      .fetchUsersByProject(organizationStore.currentOrganization!.id)
      .then(() => setLoading(false))
      .catch((error) => {
        console.error("Failed to fetch projects:", error);
        setLoading(false);
      });
  }, []);

  const [loading, setLoading] = React.useState(true);

  const handleAssign = async () => {
    await organizationStore.fetchUsersByOrganization();

    modalsStore.open("AssignUserToProjectDialog", {
      users: organizationStore.users.filter((x) => !projectStore.users.some((u) => u.id === x.id)),
      onAssign: (userId: number) =>
        projectStore.assignUserToProject(organizationStore.currentOrganization!.id, userId),
    });
  };

  const handleUnassign = (user: UserModel) => {
    sharedModalsStore.open("ConfirmModal", {
      title: "Are you sure you want to unassign this user?",
      description: `This action cannot be undone. User: ${user.fullName}`,
      onConfirm: () =>
        projectStore.unassignUserFromProject(organizationStore.currentOrganization!.id, user.id),
    });
  };

  return (
    <Flex flex="1" direction="row" width="100%">
      <ProjectSidebar />

      <Flex direction="column" width="100%" p={4}>
        {loading ? (
          <div>loading users...</div>
        ) : (
          <>
            <Flex justify="flex-end">
              <Button variant="outline" onClick={handleAssign}>
                Add User
              </Button>
            </Flex>

            <UsersList users={projectStore.users} onUnassign={handleUnassign} />
          </>
        )}
      </Flex>
    </Flex>
  );
});

export default ProjectUsers;
