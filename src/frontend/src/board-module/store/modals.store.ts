import {
  ModalsFactory,
  type ModalsPropsBase,
  type ModalsStoreRegistryGuard,
} from "src/modals-module";

import type {
  CreateOrEditBoardDialogProps,
  CreateOrEditTaskDialogProps,
} from "../components/modals";

export type ModalName = "CreateOrEditBoardDialog" | "CreateOrEditTaskDialog";

export interface IModalsStoreRegistry extends ModalsStoreRegistryGuard<ModalName> {
  CreateOrEditBoardDialog: Omit<CreateOrEditBoardDialogProps, keyof ModalsPropsBase>;
  CreateOrEditTaskDialog: Omit<CreateOrEditTaskDialogProps, keyof ModalsPropsBase>;
}

export const modalsStore = ModalsFactory.createStore<ModalName, IModalsStoreRegistry>();
