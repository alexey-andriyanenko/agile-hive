import type { TaskBoardColumnModel } from "./task-board-column.ts";
import type { TaskUserModel } from "./task-user.ts";

export type TaskModel = {
  id: string;
  title: string;
  description?: string;
  tenantId: string;
  projectId: string;
  boardId: string;
  boardColumn: TaskBoardColumnModel;
  createdBy: TaskUserModel;
  assignedTo: TaskUserModel | null;
  createdAt: Date;
  updatedAt?: Date;
};
