import React from "react";
import { observer } from "mobx-react-lite";
import { ProjectSidebar } from "src/project-module/components/project-sidebar";
import { Flex, Heading } from "@chakra-ui/react";
import { BoardsList } from "src/board-module/pages/boards/boards-list";
import { useBoardStore } from "src/board-module/store";
import { useOrganizationStore } from "src/organization-module/store";
import { useProjectStore } from "src/project-module/store";
import type { BoardModel } from "src/board-module/models";

const Boards: React.FC = observer(() => {
  const organizationStore = useOrganizationStore();
  const projectStore = useProjectStore();
  const boardsStore = useBoardStore();
  const [loading, setLoading] = React.useState(false);

  React.useEffect(() => {
    if (boardsStore.boards.length === 0) {
      setLoading(true);
      boardsStore
        .fetchBoards({
          organizationId: organizationStore.currentOrganization!.id,
          projectId: projectStore.currentProject!.id,
        })
        .catch((error) => {
          console.error("Failed to fetch boards:", error);
        })
        .finally(() => setLoading(false));
    }
  }, []);

  const handleVisit = (board: BoardModel) => {};

  const handleEdit = (board: BoardModel) => {};

  const handleDelete = (board: BoardModel) => {
    // boardsStore.deleteBoard({
    //   organizationId: organizationStore.currentOrganization!.id,
    //   projectId: projectStore.currentProject!.id,
    //   boardId: board.id,
    // }).catch((error) => {
    //   console.error("Failed to delete board:", error);
    // });
  };

  return (
    <Flex flex="1" direction="row" width="100%" height="100%">
      <ProjectSidebar />

      <Flex direction="column" width="100%" p={4} gap={4}>
        <Heading> Project Boards </Heading>

        {loading ? (
          <div>loading boards...</div>
        ) : (
          <BoardsList
            boards={boardsStore.boards}
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
