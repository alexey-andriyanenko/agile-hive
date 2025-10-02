export type BoardFormValues = {
  name: string;
  // array because chakra ui select component works only with arrays
  typeId: string[];
  columns: BoardColumnFormItem[];
};

export type BoardColumnFormItem = {
  id?: string;
  name: string;
};
