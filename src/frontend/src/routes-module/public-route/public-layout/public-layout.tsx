import React from "react";
import { observer } from "mobx-react-lite";
import { Flex } from "@chakra-ui/react";

export interface IPublicRouteProps {
  children: React.ReactNode;
}
export const PublicRoute: React.FC<IPublicRouteProps> = observer(({ children }) => {
  return (
    <Flex flex="1" className="public-layout">
      {children}
    </Flex>
  );
});
