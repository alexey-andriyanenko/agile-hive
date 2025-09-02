import {
  ModalsFactory,
  type ModalsPropsBase,
  type ModalsStoreRegistryGuard,
} from "src/modals-module";

import type { CreateOrEditProjectDialogProps } from "src/project-module/components/modals/create-or-edit-project-dialog";

export type ModalName = "CreateOrEditProjectDialog";

export interface IModalsStoreRegistry extends ModalsStoreRegistryGuard<ModalName> {
  CreateOrEditProjectDialog: Omit<CreateOrEditProjectDialogProps, keyof ModalsPropsBase>;
}

export const modalsStore = ModalsFactory.createStore<ModalName, IModalsStoreRegistry>();
