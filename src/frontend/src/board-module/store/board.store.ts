import { makeAutoObservable, runInAction } from "mobx";
import type { BoardModel } from "src/board-module/models";
import {
  boardApiService,
  type CreateBoardRequest,
  type DeleteBoardRequest,
  type GetBoardByIdRequest,
  type GetManyBoardsRequest,
  type UpdateBoardRequest,
} from "src/board-module/api";

class BoardStore {
  private _boards: BoardModel[] = [];

  private _currentBoard: BoardModel | null = null;

  constructor() {
    makeAutoObservable(this);
  }

  public get boards() {
    return this._boards;
  }

  public get currentBoard() {
    return this._currentBoard;
  }

  public setCurrentBoard(board?: BoardModel) {
    this._currentBoard = board ?? null;
  }

  public async fetchCurrentBoardById(data: GetBoardByIdRequest) {
    const res = await boardApiService.getBoardById(data);

    runInAction(() => {
      this._currentBoard = res;
    });
  }

  public async fetchBoards(data: GetManyBoardsRequest) {
    const res = await boardApiService.getManyBoards(data);

    runInAction(() => {
      this._boards = res.boards;
    });
  }

  public async createBoard(data: CreateBoardRequest) {
    const res = await boardApiService.createBoard(data);

    runInAction(() => {
      this._boards.push(res);
      this._currentBoard = res;
    });
  }

  public async updateBoard(data: UpdateBoardRequest) {
    const res = await boardApiService.updateBoard(data);

    runInAction(() => {
      const index = this._boards.findIndex((b) => b.id === res.id);
      if (index !== -1) {
        this._boards[index] = res;
      }

      if (this._currentBoard?.id === res.id) {
        this._currentBoard = res;
      }
    });
  }

  public async deleteBoard(data: DeleteBoardRequest) {
    await boardApiService.deleteBoard(data);

    runInAction(() => {
      this._boards = this._boards.filter((b) => b.id !== data.boardId);

      if (this._currentBoard?.id === data.boardId) {
        this._currentBoard = null;
      }
    });
  }
}

export const boardStore = new BoardStore();
