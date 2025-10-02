import React from "react";
import { observer } from "mobx-react-lite";
import { ModalsProvider } from "src/board-module/providers/modals";
import { useOrganizationStore } from "src/organization-module/store";
import { useProjectStore } from "src/project-module/store";
import { useBoardStore } from "src/board-module/store";

export const Providers: React.FC<React.PropsWithChildren> = observer(({ children }) => {
  const organizationStore = useOrganizationStore();
  const projectStore = useProjectStore();
  const boardStore = useBoardStore();

  React.useEffect(() => {
    if (boardStore.boardTypes.length !== 0) {
      return;
    }

    boardStore
      .fetchBoardTypes({
        organizationId: organizationStore.currentOrganization!.id,
        projectId: projectStore.currentProject!.id,
      })
      .catch((error) => {
        console.error("Failed to fetch board types:", error);
      });
  }, [boardStore, organizationStore.currentOrganization, projectStore.currentProject]);

  return (
    <>
      <ModalsProvider />
      {children}
    </>
  );
});
