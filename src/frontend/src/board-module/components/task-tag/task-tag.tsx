import React from "react";

import { Box } from "@chakra-ui/react";
import type { TaskTagModel } from "src/board-module/models";

type TagProps = {
  tag: TaskTagModel;
};

export const TaskTag: React.FC<TagProps> = ({ tag }) => {
  return (
    <Box
      px="2"
      py="1"
      borderRadius="md"
      bg={tag.color}
      color="white"
      fontSize="sm"
      fontWeight="bold"
    >
      {tag.name}
    </Box>
  );
};
