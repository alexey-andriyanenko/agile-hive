import React from "react";
import { observer } from "mobx-react-lite";

export interface IPublicRouteProps {
  children: React.ReactNode;
}
export const PublicRoute: React.FC<IPublicRouteProps> = observer(({ children }) => {
  return <div className="public-layout">{children}</div>;
});
