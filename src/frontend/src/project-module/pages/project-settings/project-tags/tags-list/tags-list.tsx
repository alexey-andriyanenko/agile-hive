import React from "react";
import { observer } from "mobx-react-lite";
import { Grid, Card, Flex, Text, IconButton, Box } from "@chakra-ui/react";
import { LuPencil, LuTrash2 } from "react-icons/lu";

type Tag = {
  id: string;
  name: string;
  color: string;
};

type TagsListProps = {
  tags: Tag[];
  onEdit: (tagId: string) => void;
  onDelete: (tagId: string) => void;
};

export const TagsList: React.FC<TagsListProps> = observer(({ tags, onEdit, onDelete }) => {
  if (tags.length === 0) {
    return (
      <Flex justify="center" align="center" p={8}>
        <Text color="gray.500">No tags found. Create your first tag to get started.</Text>
      </Flex>
    );
  }

  return (
    <Grid templateColumns="repeat(auto-fill, minmax(280px, 1fr))" gap={4}>
      {tags.map((tag) => (
        <Card.Root key={tag.id} size="sm">
          <Card.Body>
            <Flex justify="space-between" align="center">
              <Flex align="center" gap={3}>
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
              </Flex>

              <Flex gap={2}>
                <IconButton
                  aria-label="Edit tag"
                  variant="ghost"
                  size="sm"
                  onClick={() => onEdit(tag.id)}
                >
                  <LuPencil />
                </IconButton>
                <IconButton
                  aria-label="Delete tag"
                  variant="ghost"
                  size="sm"
                  colorPalette="red"
                  onClick={() => onDelete(tag.id)}
                >
                  <LuTrash2 />
                </IconButton>
              </Flex>
            </Flex>
          </Card.Body>
        </Card.Root>
      ))}
    </Grid>
  );
});
