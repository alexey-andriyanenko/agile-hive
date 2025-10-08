import { makeAutoObservable } from "mobx";

import { taskApiService } from "src/board-module/api";
import type {
  GetTaskByIdRequest,
  GetTasksByBoardIdRequest,
  CreateTaskRequest,
  UpdateTaskRequest,
  DeleteTaskRequest,
} from "src/board-module/api";
import type { TaskModel } from "src/board-module/models";

class TaskStore {
  private _tasks: TaskModel[] = [];

  constructor() {
    makeAutoObservable(this);
  }

  public get tasks() {
    return this._tasks;
  }

  public get tasksByColumnId(): Record<string, TaskModel[]> {
    return this._tasks.reduce(
      (acc, task) => {
        if (!acc[task.boardColumn.id]) {
          acc[task.boardColumn.id] = [];
        }

        acc[task.boardColumn.id].push(task);

        return acc;
      },
      {} as Record<string, TaskModel[]>,
    );
  }

  public async fetchTaskById(data: GetTaskByIdRequest) {
    const res = await taskApiService.getTaskById(data);
    const index = this._tasks.findIndex((t) => t.id === res.id);

    if (index > -1) {
      this._tasks[index] = res;
    }
  }

  public async fetchTasksByBoardId(data: GetTasksByBoardIdRequest) {
    const res = await taskApiService.getTasksByBoardId(data);
    this._tasks = res.tasks;
  }

  public async createTask(data: CreateTaskRequest) {
    const res = await taskApiService.createTask(data);
    this._tasks.push(res);
  }

  public async updateTask(data: UpdateTaskRequest) {
    const res = await taskApiService.updateTask(data);
    const index = this._tasks.findIndex((t) => t.id === res.id);

    if (index > -1) {
      this._tasks[index] = res;
    }
  }

  public async deleteTask(data: DeleteTaskRequest) {
    await taskApiService.deleteTask(data);
    this._tasks = this._tasks.filter((t) => t.id !== data.taskId);
  }
}

export const taskStore = new TaskStore();
