import type { RouteItem } from "src/routes-module/routes-list/routes-list.types.ts";

import SignIn from "src/auth-module/sign-in";
import SignUp from "src/auth-module/sign-up";

export const AuthRoutes = {
  login: "/auth/login",
  register: "/auth/register",
};

const authRoutes: RouteItem[] = [
  {
    path: AuthRoutes.login,
    element: <SignIn />,
  },
  {
    path: AuthRoutes.register,
    element: <SignUp />,
  },
];

export default authRoutes;
