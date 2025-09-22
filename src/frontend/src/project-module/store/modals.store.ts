import {
  ModalsFactory,
  type ModalsPropsBase,
  type ModalsStoreRegistryGuard,
} from "src/modals-module";

import type {
  CreateOrEditProjectDialogProps,
  AddUsersToProjectDialogProps,
} from "src/project-module/components/modals";

export type ModalName = "CreateOrEditProjectDialog" | "AddUsersToProjectDialog";

export interface IModalsStoreRegistry extends ModalsStoreRegistryGuard<ModalName> {
  CreateOrEditProjectDialog: Omit<CreateOrEditProjectDialogProps, keyof ModalsPropsBase>;
  AddUsersToProjectDialog: Omit<AddUsersToProjectDialogProps, keyof ModalsPropsBase>;
}

export const modalsStore = ModalsFactory.createStore<ModalName, IModalsStoreRegistry>();
