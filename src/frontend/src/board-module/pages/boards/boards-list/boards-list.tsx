import React from "react";
import { observer } from "mobx-react-lite";
import { Table, Menu, IconButton, Portal } from "@chakra-ui/react";
import { HiDotsVertical } from "react-icons/hi";

import { BOARDS_LIST_COLUMNS } from "./boards-list.constants.ts";
import type { BoardModel } from "../../../models/board.ts";

type BoardsListProps = {
  boards: BoardModel[];
  onVisit: (board: BoardModel) => void;
  onEdit: (board: BoardModel) => void;
  onDelete: (user: BoardModel) => void;
};

export const BoardsList: React.FC<BoardsListProps> = observer(
  ({ boards, onEdit, onDelete, onVisit }) => {
    const handleMenu = (manuItem: string, board: BoardModel) => {
      if (manuItem === "edit") {
        onEdit(board);
      }

      if (manuItem === "delete") {
        onDelete(board);
      }

      if (manuItem === "visit") {
        onVisit(board);
      }
    };

    const getCellValue = (board: BoardModel, key: string): string => {
      if (key == "type") {
        return board.type.name.toUpperCase();
      }

      const val = board[key as keyof BoardModel];

      if (val instanceof Date) {
        return val.toLocaleDateString();
      }

      return (val as string) || "N/A";
    };

    return (
      <Table.Root>
        <Table.Header>
          <Table.Row>
            {BOARDS_LIST_COLUMNS.map((col) => (
              <Table.ColumnHeader key={col.key}>{col.label}</Table.ColumnHeader>
            ))}

            <Table.ColumnHeader>Actions</Table.ColumnHeader>
          </Table.Row>
        </Table.Header>

        <Table.Body>
          {boards.map((board) => (
            <Table.Row key={board.id}>
              {BOARDS_LIST_COLUMNS.map((col) => (
                <Table.Cell key={col.key}>{getCellValue(board, col.key)}</Table.Cell>
              ))}

              <Table.Cell>
                <Menu.Root onSelect={({ value }) => handleMenu(value, board)}>
                  <Menu.Trigger asChild>
                    <IconButton variant="outline">
                      <HiDotsVertical />
                    </IconButton>
                  </Menu.Trigger>
                  <Portal>
                    <Menu.Positioner>
                      <Menu.Content>
                        <Menu.Item value="visit">Visit</Menu.Item>
                        <Menu.Item value="edit">Edit</Menu.Item>
                        <Menu.Item value="delete">Delete</Menu.Item>
                      </Menu.Content>
                    </Menu.Positioner>
                  </Portal>
                </Menu.Root>
              </Table.Cell>
            </Table.Row>
          ))}
        </Table.Body>
      </Table.Root>
    );
  },
);
