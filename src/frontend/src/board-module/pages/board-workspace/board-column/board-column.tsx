import React from "react";
import { observer } from "mobx-react-lite";
import { Heading, Stack } from "@chakra-ui/react";
import type { BoardColumnModel } from "src/board-module/models";

type BoardColumnProps = {
  column: BoardColumnModel;
};

export const BoardColumn: React.FC<BoardColumnProps> = observer(({ column }) => {
  return (
    <Stack
      width="300px"
      minWidth="300px"
      height="100%"
      padding={4}
      borderWidth="1px"
      borderColor="gray.200"
    >
      <Stack direction="row" alignItems="center" justifyContent="space-between">
        <Heading>{column.name}</Heading>
      </Stack>
    </Stack>
  );
});
