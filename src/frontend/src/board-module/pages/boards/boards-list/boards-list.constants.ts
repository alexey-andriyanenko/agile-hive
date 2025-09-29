import type { BoardsListColumn } from "./boards-list.types.ts";

export const BOARDS_LIST_COLUMNS: BoardsListColumn[] = [
  {
    key: "name",
    label: "Name",
  },
  {
    key: "type",
    label: "Type",
  },
  {
    key: "createdAt",
    label: "Created At",
  },
  {
    key: "updatedAt",
    label: "Updated At",
  },
];
