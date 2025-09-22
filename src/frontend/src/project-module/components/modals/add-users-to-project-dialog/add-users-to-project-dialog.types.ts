export type AddUserToProjectFormValues = {
  users: AddUserToProjectFormItem[];
};

export type AddUserToProjectFormItem = {
  // arrays are used because ChakraUI select only works with array even if it's single select
  userId: string[];
  role: string[];
};
