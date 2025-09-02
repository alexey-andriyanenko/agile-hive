import React from "react";
import { observer } from "mobx-react-lite";

export interface IPrivateRouteProps {
  children: React.ReactNode;
}
export const PrivateRoute: React.FC<IPrivateRouteProps> = observer(({ children }) => {
  return <div className="private-layout">{children}</div>;
});
