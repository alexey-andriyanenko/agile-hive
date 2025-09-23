export type ProjectFormValues = {
  name: string;
  description: string;
  // arrays because chakra select works only with arrays even in single select mode
  visibility: string[];
  users: AddUserToProjectItem[];
};

export type AddUserToProjectItem = {
  // arrays because chakra select works only with arrays even in single select mode
  userId: string[];
  role: string[];
};
