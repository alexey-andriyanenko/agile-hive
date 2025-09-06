import React from "react";
import { observer } from "mobx-react-lite";
import { ModalsProvider } from "./modals";

export const Providers: React.FC<React.PropsWithChildren> = observer(({ children }) => {
  return (
    <>
      <ModalsProvider />
      {children}
    </>
  );
});
