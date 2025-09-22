import React from "react";
import { observer } from "mobx-react-lite";
import { Flex } from "@chakra-ui/react";
import { useAuthStore } from "src/auth-module/store";
import { useNavigate } from "react-router-dom";
import { AuthRoutes } from "src/auth-module";

export interface IPrivateRouteProps {
  children: React.ReactNode;
}
export const PrivateRoute: React.FC<IPrivateRouteProps> = observer(({ children }) => {
  const authStore = useAuthStore();
  const navigate = useNavigate();
  const [loading, setLoading] = React.useState(true);

  React.useEffect(() => {
    if (authStore.currentUser) {
      setLoading(false);
      return;
    }

    authStore
      .loadMe()
      .then(() => {
        setLoading(false);
      })
      .catch(() => {
        setLoading(false);
        navigate(AuthRoutes.login);
      });
  }, [authStore, navigate]);

  return loading ? (
    <Flex width="100vw" height="100vh" alignItems="center" justifyContent="center">
      Loading...
    </Flex>
  ) : (
    <Flex flex="1" className="private-layout">
      {children}
    </Flex>
  );
});
