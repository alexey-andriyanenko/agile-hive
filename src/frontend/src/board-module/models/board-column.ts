export type BoardColumnModel = {
  id: string;
  name: string;
  order: number;
  boardId: string;
  createdByUserId: string;
  createdAt: Date;
  updatedAt: Date | null;
};
