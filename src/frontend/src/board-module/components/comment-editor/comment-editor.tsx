import React, { useState } from "react";
import { LuPencil, LuCheck, LuTrash } from "react-icons/lu";
import { Button, Flex, Stack, Text } from "@chakra-ui/react";

import { clsx } from "src/shared-module/utils";
import { TipTapEditor } from "../tiptap-editor";
import { TipTapEditorContent } from "../tiptap-editor-content";

import "./comment-editor.css";

type CommentEditorProps = {
  createdAt?: string;
  updatedAt?: string;
  value: string;
  onChange?: (value: string) => void;
  readonly?: boolean;
  onSubmit?: (value: string) => void;
  onDelete?: () => void;
};

export const CommentEditor: React.FC<CommentEditorProps> = ({
  createdAt,
  updatedAt,
  value,
  onChange,
  readonly,
  onDelete,
}) => {
  const [editable, setEditable] = useState(false);

  const handleEdit = () => setEditable(true);

  const handleSubmit = () => {
    setEditable(false);
  };

  const hasDates = Boolean(createdAt || updatedAt);

  return (
    <Stack width="100%" gap={2} className={clsx("comment-editor", { ["editable"]: editable })}>
      {editable ? (
        <TipTapEditor value={value} onChange={onChange!} className="comment-editor-content" />
      ) : (
        <TipTapEditorContent content={value} className="comment-editor-content" />
      )}

      <Flex gap={2} justifyContent={hasDates ? "space-between" : "flex-end"} alignItems="center">
        {createdAt || updatedAt ? (
          <Text fontSize="xs" color="gray.500">
            {createdAt && `Created at: ${new Date(createdAt).toLocaleString()}`}
            {createdAt && updatedAt && " | "}
            {updatedAt && `Updated at: ${new Date(updatedAt).toLocaleString()}`}
          </Text>
        ) : null}

        {!readonly && (
          <Button size="sm" variant="outline" colorPalette="red" onClick={onDelete}>
            <LuTrash />
            Delete
          </Button>
        )}

        {!readonly && !editable && (
          <Button size="sm" variant="outline" onClick={handleEdit}>
            <LuPencil />
            Edit
          </Button>
        )}

        {editable && (
          <Button size="sm" onClick={handleSubmit} colorScheme="blue">
            <LuCheck />
            Submit
          </Button>
        )}
      </Flex>
    </Stack>
  );
};
