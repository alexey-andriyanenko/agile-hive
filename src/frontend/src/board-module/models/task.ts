import type { TaskBoardColumnModel } from "./task-board-column.ts";
import type { TaskUserModel } from "./task-user.ts";
import type { TaskTagModel } from "./task-tag.ts";

export type TaskModel = {
  id: string;
  title: string;
  description: string;
  tenantId: string;
  projectId: string;
  boardId: string;
  boardColumn: TaskBoardColumnModel;
  createdBy: TaskUserModel;
  assignedTo: TaskUserModel | null;
  tags: TaskTagModel[];
  createdAt: Date;
  updatedAt?: Date;
};
