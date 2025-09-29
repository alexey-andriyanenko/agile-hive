import React from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useProjectStore } from "src/project-module/store";
import { useOrganizationStore } from "src/organization-module/store";
import { useBoardStore } from "src/board-module/store";

const BoardRouteGuard: React.FC<React.PropsWithChildren> = ({ children }) => {
  const navigate = useNavigate();
  const routeParams = useParams<{ boardId: string }>();
  const organizationStore = useOrganizationStore();
  const projectStore = useProjectStore();
  const boardStore = useBoardStore();
  const [verifyingBoard, setVerifyingBoard] = React.useState(true);

  React.useEffect(() => {
    if (!routeParams.boardId) {
      return;
    }

    if (boardStore.currentBoard && boardStore.currentBoard.id === routeParams.boardId) {
      setVerifyingBoard(false);
      return;
    }

    boardStore
      .fetchCurrentBoardById({
        organizationId: organizationStore.currentOrganization!.id,
        projectId: projectStore.currentProject!.id,
        boardId: routeParams.boardId,
      })
      .then(() => setVerifyingBoard(false))
      .catch((error) => {
        navigate(
          `/organization/${organizationStore.currentOrganization!.slug}/project/${projectStore.currentProject!.slug}/invalid-board`,
        );
        console.error("Failed to fetch board by ID:", error);
      });
  }, [routeParams.boardId, boardStore, organizationStore, projectStore, navigate]);

  return verifyingBoard ? <div>Verifying board...</div> : children;
};

export default BoardRouteGuard;
