import type { TaskModel } from "../models";

export type TaskResponseModel = {
  id: string;
  title: string;
  description: string;
  tenantId: string;
  projectId: string;
  boardId: string;
  boardColumn: TaskBoardColumnResponseModel;
  createdBy: TaskUserResponseModel;
  assignedTo: TaskUserResponseModel | null;
  createdAt: Date;
  updatedAt?: Date;
  tags: TagResponseModel[];
};

export type TaskBoardColumnResponseModel = {
  id: string;
  name: string;
};

export type TaskUserResponseModel = {
  id: string;
  fullName: string;
  email: string;
};

export type TagResponseModel = {
  id: string;
  name: string;
  color: string;
};

export type GetTagsByProjectIdRequest = {
  projectId: string;
  organizationId: string;
};

export type GetTagsByProjectIdResponse = {
  tags: TagResponseModel[];
};

export type GetTaskByIdRequest = {
  taskId: string;
  projectId: string;
  organizationId: string;
};

export type GetTaskByIdResponse = TaskModel;

export type GetTasksByBoardIdRequest = {
  boardId: string;
  projectId: string;
  organizationId: string;
};

export type GetTasksByBoardIdResponse = {
  tasks: TaskModel[];
};

export type CreateTaskRequest = {
  organizationId: string;
  projectId: string;
  boardId: string;
  title: string;
  description: string;
  boardColumnId: string;
  assigneeUserId?: string;
  tagIds: string[];
};

export type CreateTaskResponse = TaskModel;

export type UpdateTaskRequest = {
  taskId: string;
  organizationId: string;
  projectId: string;
  boardId: string;
  title: string;
  description: string;
  boardColumnId?: string;
  assigneeUserId?: string | null;
  tagIds: string[];
};

export type UpdateTaskResponse = TaskModel;

export type DeleteTaskRequest = {
  taskId: string;
  organizationId: string;
  projectId: string;
  boardId: string;
};
