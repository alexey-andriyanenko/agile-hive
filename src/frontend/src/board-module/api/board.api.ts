import { appHttpClient } from "src/shared-module/api";
import type {
  BoardColumnResponseModel,
  BoardResponseModel,
  BoardTypeResponseModel,
  CreateBoardRequest,
  CreateBoardResponse,
  GetBoardByIdRequest,
  GetBoardByIdResponse,
  GetManyBoardsRequest,
  GetManyBoardsResponse,
  GetManyBoardTypesRequest,
  GetManyBoardTypesResponse,
  UpdateBoardRequest,
  UpdateBoardResponse,
} from "./board.types";
import type { BoardColumnModel, BoardModel, BoardTypeModel } from "../models";

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

  async getBoardTypes(data: GetManyBoardTypesRequest): Promise<GetManyBoardTypesResponse> {
    const res = await appHttpClient
      .get<{ boardTypes: BoardTypeResponseModel[] }>(
        "/organizations/:organizationId/projects/:projectId/boards/types",
      )
      .setRouteParams({
        organizationId: data.organizationId,
        projectId: data.projectId,
      })
      .send();

    return {
      boardTypes: res.boardTypes.map(BoardApiService.toBoardTypeDomain),
    };
  }

  private static toBoardDomain(res: BoardResponseModel): BoardModel {
    return {
      ...res,
      createdAt: new Date(res.createdAt),
      updatedAt: res.updatedAt ? new Date(res.updatedAt) : null,
      columns: res.columns.map(BoardApiService.toBoardColumnDomain),
    };
  }

  private static toBoardColumnDomain(res: BoardColumnResponseModel): BoardColumnModel {
    return {
      ...res,
      createdAt: new Date(res.createdAt),
      updatedAt: res.updatedAt ? new Date(res.updatedAt) : null,
    };
  }

  private static toBoardTypeDomain(res: BoardTypeResponseModel): BoardTypeModel {
    return {
      id: res.id,
      name: res.name,
      isEssential: res.isEssential,
    };
  }
}

export const boardApiService = new BoardApiService();
