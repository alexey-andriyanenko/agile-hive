export type UserModel = {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: "ADMIN" | "USER";
};
