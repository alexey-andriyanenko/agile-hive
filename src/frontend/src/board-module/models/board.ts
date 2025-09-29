import type { BoardTypeModel } from "./board-type.ts";
import type { BoardColumnModel } from "./board-column.ts";

export type BoardModel = {
  id: string;
  name: string;
  projectId: string;
  type: BoardTypeModel;
  columns: BoardColumnModel[];
  createdByUserId: string;
  createdAt: Date;
  updatedAt: Date | null;
};
