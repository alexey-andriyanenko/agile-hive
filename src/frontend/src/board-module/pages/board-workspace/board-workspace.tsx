import React from "react";
import { observer } from "mobx-react-lite";
import { Flex } from "@chakra-ui/react";

import { useBoardStore, useTaskStore } from "src/board-module/store";
import { BoardColumn } from "src/board-module/pages/board-workspace/board-column";
import { ProjectSidebar } from "src/project-module/components/project-sidebar";
import { useOrganizationStore } from "src/organization-module/store";
import { useTagStore } from "src/project-module/store";

const BoardWorkspace: React.FC = observer(() => {
  const organizationStore = useOrganizationStore();
  const boardStore = useBoardStore();
  const taskStore = useTaskStore();
  const tagStore = useTagStore();
  const [loadingTasks, setLoadingTasks] = React.useState(false);
  const [loadingTags, setLoadingTags] = React.useState(false);

  React.useEffect(() => {
    setLoadingTasks(true);
    taskStore
      .fetchTasksByBoardId({
        organizationId: organizationStore.currentOrganization!.id,
        projectId: boardStore.currentBoard!.projectId,
        boardId: boardStore.currentBoard!.id,
      })
      .catch((error) => {
        console.error("Failed to fetch tasks for the board:", error);
      })
      .finally(() => setLoadingTasks(false));
  }, [boardStore.currentBoard, organizationStore, boardStore, taskStore]);

  React.useEffect(() => {
    if (tagStore.tags.length !== 0) {
      return;
    }

    setLoadingTags(true);
    tagStore
      .fetchTagsByProjectId({
        organizationId: organizationStore.currentOrganization!.id,
        projectId: boardStore.currentBoard!.projectId,
      })
      .catch((error) => {
        console.error("Failed to fetch tags for the project:", error);
      })
      .finally(() => setLoadingTags(false));
  }, [boardStore.currentBoard, organizationStore.currentOrganization, taskStore]);

  return (
    <Flex flex="1" direction="row">
      <ProjectSidebar />

      {loadingTags || loadingTasks ? (
        <div>Loading...</div>
      ) : (
        <Flex direction="column" width="calc(100vw - 320px)" overflow="hidden" p={4}>
          <Flex flex="1" width="100%" gap={4} overflowX="auto">
            {boardStore.currentBoard!.columns.map((column) => (
              <BoardColumn key={column.id} column={column} />
            ))}
          </Flex>
        </Flex>
      )}
    </Flex>
  );
});

export default BoardWorkspace;
