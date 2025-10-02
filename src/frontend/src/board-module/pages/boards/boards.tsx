import React from "react";
import { observer } from "mobx-react-lite";
import { useNavigate } from "react-router-dom";
import { Button, Flex, Heading } from "@chakra-ui/react";

import { BoardsList } from "src/board-module/pages/boards/boards-list";
import { useBoardStore } from "src/board-module/store";
import { useOrganizationStore } from "src/organization-module/store";
import { useProjectStore } from "src/project-module/store";
import { useModalsStore as useBoardModalsStore } from "src/board-module/store";
import { useModalsStore as useSharedModalsStore } from "src/shared-module/store/modals";
import type { BoardModel } from "src/board-module/models";
import { ProjectSidebar } from "src/project-module/components/project-sidebar";

const Boards: React.FC = observer(() => {
  const navigate = useNavigate();
  const organizationStore = useOrganizationStore();
  const projectStore = useProjectStore();
  const boardStore = useBoardStore();
  const boardModalsStore = useBoardModalsStore();
  const sharedModalsStore = useSharedModalsStore();
  const [loading, setLoading] = React.useState(false);

  React.useEffect(() => {
    if (boardStore.boards.length !== 0) {
      return;
    }

    setLoading(true);
    boardStore
      .fetchBoards({
        organizationId: organizationStore.currentOrganization!.id,
        projectId: projectStore.currentProject!.id,
      })
      .catch((error) => {
        console.error("Failed to fetch boards:", error);
      })
      .finally(() => setLoading(false));
  }, []);

  const handleCreate = () => {
    boardModalsStore.open("CreateOrEditBoardDialog", {
      onCreate: (data) =>
        boardStore.createBoard({
          organizationId: organizationStore.currentOrganization!.id,
          projectId: projectStore.currentProject!.id,
          name: data.name,
          boardTypeId: data.typeId[0],
          columns: data.columns.map((col) => ({
            name: col.name,
          })),
        }),
    });
  };

  const handleVisit = (board: BoardModel) => {
    navigate(
      `/organization/${organizationStore.currentOrganization!.slug}/projects/${projectStore.currentProject!.slug}/boards/${board.id}`,
    );
  };

  const handleEdit = (board: BoardModel) => {
    boardModalsStore.open("CreateOrEditBoardDialog", {
      board,
      onEdit: (data) =>
        boardStore.updateBoard({
          organizationId: organizationStore.currentOrganization!.id,
          projectId: projectStore.currentProject!.id,
          boardId: board.id,
          name: data.name,
          columns: data.columns.map((col) => ({
            id: col.id,
            name: col.name,
          })),
        }),
    });
  };

  const handleDelete = (board: BoardModel) => {
    sharedModalsStore.open("ConfirmModal", {
      title: "Are you sure you want to delete this board?",
      description: `This action cannot be undone. All tasks are going to be deleted as well. Board: ${board.name}`,
      onConfirm: boardStore.deleteBoard({
        organizationId: organizationStore.currentOrganization!.id,
        projectId: projectStore.currentProject!.id,
        boardId: board.id,
      }),
    });
  };

  return (
    <Flex flex="1" direction="row" width="100%" height="100%">
      <ProjectSidebar />

      <Flex direction="column" width="100%" p={4} gap={4}>
        <Flex justify="space-between" align="center">
          <Heading> Project Boards </Heading>

          {loading ? null : <Button onClick={handleCreate}> Create Board </Button>}
        </Flex>

        {loading ? (
          <div>Loading boards...</div>
        ) : (
          <BoardsList
            boards={boardStore.boards}
            onVisit={handleVisit}
            onEdit={handleEdit}
            onDelete={handleDelete}
          />
        )}
      </Flex>
    </Flex>
  );
});

export default Boards;
