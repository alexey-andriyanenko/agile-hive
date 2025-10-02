import type { BoardModel, BoardColumnModel, BoardTypeModel } from "../models";

export type BoardResponseModel = {
  id: string;
  name: string;
  projectId: string;
  type: BoardTypeResponseModel;
  columns: BoardColumnResponseModel[];
  createdByUserId: string;
  createdAt: string;
  updatedAt?: string;
};

export type BoardColumnResponseModel = {
  id: string;
  name: string;
  order: number;
  boardId: string;
  createdByUserId: string;
  createdAt: string;
  updatedAt?: string;
};

export type BoardTypeResponseModel = {
  id: string;
  name: string;
  isEssential: boolean;
};

export type GetManyBoardsRequest = {
  projectId: string;
  organizationId: string;
};

export type GetManyBoardsResponse = {
  boards: BoardModel[];
};

export type GetBoardByIdRequest = {
  boardId: string;
  projectId: string;
  organizationId: string;
};

export type GetBoardByIdResponse = BoardModel;

export type CreateBoardRequest = {
  projectId: string;
  organizationId: string;
  name: string;
  boardTypeId: string;
  columns: CreateBoardColumnItemRequest[];
};

export type CreateBoardColumnItemRequest = {
  name: string;
};

export type CreateBoardResponse = BoardModel;

export type UpdateBoardRequest = {
  boardId: string;
  projectId: string;
  organizationId: string;
  name: string;
  columns: CreateOrUpdateBoardColumnItemRequest[];
};

export type CreateOrUpdateBoardColumnItemRequest = {
  id?: string;
  name: string;
};

export type UpdateBoardResponse = BoardModel;

export type DeleteBoardRequest = {
  boardId: string;
  projectId: string;
  organizationId: string;
};

export type CreateBoardColumnRequest = {
  boardId: string;
  projectId: string;
  organizationId: string;
  name: string;
  order: number;
};

export type CreateBoardColumnResponse = BoardColumnModel;

export type UpdateBoardColumnRequest = {
  boardId: string;
  boardColumnId: string;
  projectId: string;
  organizationId: string;
  name: string;
  order: number;
};

export type DeleteBoardColumnRequest = {
  boardId: string;
  boardColumnId: string;
  projectId: string;
  organizationId: string;
};

export type GetManyBoardTypesRequest = {
  organizationId: string;
  projectId: string;
};

export type GetManyBoardTypesResponse = {
  boardTypes: BoardTypeModel[];
};
