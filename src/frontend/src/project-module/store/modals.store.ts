import {
  ModalsFactory,
  type ModalsPropsBase,
  type ModalsStoreRegistryGuard,
} from "src/modals-module";

import type {
  CreateProjectDialogProps,
  AddUsersToProjectDialogProps,
} from "src/project-module/components/modals";

export type ModalName = "CreateProjectDialog" | "AddUsersToProjectDialog";

export interface IModalsStoreRegistry extends ModalsStoreRegistryGuard<ModalName> {
  CreateProjectDialog: Omit<CreateProjectDialogProps, keyof ModalsPropsBase>;
  AddUsersToProjectDialog: Omit<AddUsersToProjectDialogProps, keyof ModalsPropsBase>;
}

export const modalsStore = ModalsFactory.createStore<ModalName, IModalsStoreRegistry>();
