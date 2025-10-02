import { createListCollection } from "@chakra-ui/react";

export const scrumDefaultBoardColumns: string[] = [
  "To Do",
  "In Progress",
  "In Review",
  "In Testing",
  "Done",
];

export const kanbanDefaultBoardColumns: string[] = ["To Do", "In Progress", "Done"];

export const backlogDefaultBoardColumns: string[] = ["Backlog"];

export const boardTypeOptions = createListCollection({
  items: [
    { label: "Kanban", value: "kanban" },
    { label: "Scrum", value: "scrum" },
    { label: "Backlog", value: "backlog" },
    {},
  ],
});
