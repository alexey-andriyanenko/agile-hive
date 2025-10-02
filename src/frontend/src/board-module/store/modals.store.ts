import {
  ModalsFactory,
  type ModalsPropsBase,
  type ModalsStoreRegistryGuard,
} from "src/modals-module";

import type { CreateOrEditBoardDialogProps } from "../components/modals";

export type ModalName = "CreateOrEditBoardDialog";

export interface IModalsStoreRegistry extends ModalsStoreRegistryGuard<ModalName> {
  CreateOrEditBoardDialog: Omit<CreateOrEditBoardDialogProps, keyof ModalsPropsBase>;
}

export const modalsStore = ModalsFactory.createStore<ModalName, IModalsStoreRegistry>();
