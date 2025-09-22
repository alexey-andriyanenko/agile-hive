import type { UserColumn } from "./user-list.types.ts";

export const USERS_LIST_COLUMNS: UserColumn[] = [
  {
    key: "firstName",
    label: "First Name",
  },
  {
    key: "lastName",
    label: "Last Name",
  },
  {
    key: "userName",
    label: "Username",
  },
  {
    key: "email",
    label: "Email",
  },
  {
    key: "role",
    label: "Role",
  },
];
