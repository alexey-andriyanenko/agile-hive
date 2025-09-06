import React from "react";
import { observer } from "mobx-react-lite";
import { useAuthStore } from "src/auth-module/store";
import { Navigate } from "react-router-dom";
import { AuthRoutes } from "src/auth-module";

const Index: React.FC = observer(() => {
  const authStore = useAuthStore();

  if (!authStore.isLogged) {
    return <Navigate to={AuthRoutes.login} />;
  }

  return (
    <div>
      <h1>Welcome back, user</h1>
    </div>
  );
});

export default Index;
