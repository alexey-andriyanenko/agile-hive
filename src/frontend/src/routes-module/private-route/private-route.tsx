import React from "react";
import { observer } from "mobx-react-lite";
import { Flex } from "@chakra-ui/react";

export interface IPrivateRouteProps {
  children: React.ReactNode;
}
export const PrivateRoute: React.FC<IPrivateRouteProps> = observer(({ children }) => {
  return (
    <Flex flex="1" className="private-layout">
      {children}
    </Flex>
  );
});
