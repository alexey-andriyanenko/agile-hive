import { ModalsFactory } from "src/modals-module";
import type { ModalsPropsBase, ModalsStoreRegistryGuard } from "src/modals-module";

import type {
  CreateOrEditOrganizationDialogProps,
  CreateOrEditOrganizationUserDialogProps,
} from "src/organization-module/components/modals";

export type ModalName = "CreateOrEditOrganizationDialog" | "CreateOrEditOrganizationUserDialog";

export interface IModalsStoreRegistry extends ModalsStoreRegistryGuard<ModalName> {
  CreateOrEditOrganizationDialog: Omit<CreateOrEditOrganizationDialogProps, keyof ModalsPropsBase>;
  CreateOrEditOrganizationUserDialog: Omit<
    CreateOrEditOrganizationUserDialogProps,
    keyof ModalsPropsBase
  >;
}

export const modalsStore = ModalsFactory.createStore<ModalName, IModalsStoreRegistry>();
