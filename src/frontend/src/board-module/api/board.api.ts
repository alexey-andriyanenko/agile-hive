import { appHttpClient } from "src/shared-module/api";
import type {
  BoardColumnResponseModel,
  BoardResponseModel,
  CreateBoardRequest,
  CreateBoardResponse,
  GetBoardByIdRequest,
  GetBoardByIdResponse,
  GetManyBoardsRequest,
  GetManyBoardsResponse,
  UpdateBoardRequest,
  UpdateBoardResponse,
} from "./board.types";
import type { BoardColumnModel, BoardModel } from "../models";

class BoardApiService {
  async getManyBoards(data: GetManyBoardsRequest): Promise<GetManyBoardsResponse> {
    const res = await appHttpClient
      .get<{ boards: BoardResponseModel[] }>(
        "/organizations/:organizationId/projects/:projectId/boards",
      )
      .setRouteParams({
        organizationId: data.organizationId,
        projectId: data.projectId,
      })
      .send();

    return {
      boards: res.boards.map(BoardApiService.toBoardDomain),
    };
  }

  async getBoardById(data: GetBoardByIdRequest): Promise<GetBoardByIdResponse> {
    const res = await appHttpClient
      .get<BoardResponseModel>("/organizations/:organizationId/projects/:projectId/boards/:boardId")
      .setRouteParams({
        organizationId: data.organizationId,
        projectId: data.projectId,
        boardId: data.boardId,
      })
      .send();

    return BoardApiService.toBoardDomain(res);
  }

  async createBoard(data: CreateBoardRequest): Promise<CreateBoardResponse> {
    const res = await appHttpClient
      .post<CreateBoardRequest, BoardResponseModel>(
        "/organizations/:organizationId/projects/:projectId/boards",
      )
      .setRouteParams({
        organizationId: data.organizationId,
        projectId: data.projectId,
      })
      .send(data);

    return BoardApiService.toBoardDomain(res);
  }

  async updateBoard(data: UpdateBoardRequest): Promise<UpdateBoardResponse> {
    const res = await appHttpClient
      .put<UpdateBoardRequest, BoardResponseModel>(
        "/organizations/:organizationId/projects/:projectId/boards/:boardId",
      )
      .setRouteParams({
        organizationId: data.organizationId,
        projectId: data.projectId,
        boardId: data.boardId,
      })
      .send(data);

    return BoardApiService.toBoardDomain(res);
  }

  async deleteBoard(data: GetBoardByIdRequest): Promise<void> {
    await appHttpClient
      .delete<void>("/organizations/:organizationId/projects/:projectId/boards/:boardId")
      .setRouteParams({
        organizationId: data.organizationId,
        projectId: data.projectId,
        boardId: data.boardId,
      })
      .send();
  }

  private static toBoardDomain(res: BoardResponseModel): BoardModel {
    return {
      ...res,
      createdAt: new Date(res.createdAt),
      updatedAt: res.updatedAt ? new Date(res.updatedAt) : null,
      columns: res.columns.map(this.toBoardColumnDomain),
    };
  }

  private static toBoardColumnDomain(res: BoardColumnResponseModel): BoardColumnModel {
    return {
      ...res,
      createdAt: new Date(res.createdAt),
      updatedAt: res.updatedAt ? new Date(res.updatedAt) : null,
    };
  }
}

export const boardApiService = new BoardApiService();
