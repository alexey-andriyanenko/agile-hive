export type TaskFormValues = {
  title: string;
  description: JSON;
  tagIds: string[];

  // array because chakra ui select works with arrays
  boardColumnId: string[];
  assigneeId: string[];

  comments: CommentFormItem[];
};

export type CommentFormItem = {
  id?: string;
  createdByUserId?: string;
  content: string;
  createdAt?: string;
  updatedAt?: string;
};
