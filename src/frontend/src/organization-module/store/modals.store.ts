import { ModalsFactory } from "src/modals-module";
import type { ModalsPropsBase, ModalsStoreRegistryGuard } from "src/modals-module";

import type { CreateOrEditOrganizationDialogProps } from "src/organization-module/components/modals/create-or-edit-organization-dialog";

export type ModalName = "CreateOrEditOrganizationDialog";

export interface IModalsStoreRegistry extends ModalsStoreRegistryGuard<ModalName> {
  CreateOrEditOrganizationDialog: Omit<CreateOrEditOrganizationDialogProps, keyof ModalsPropsBase>;
}

export const modalsStore = ModalsFactory.createStore<ModalName, IModalsStoreRegistry>();
