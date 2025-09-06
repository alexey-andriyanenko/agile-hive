import React from "react";
import { ModalsFactory, type ModalsProviderRegistryGuard } from "src/modals-module";

import { modalsStore } from "../../store/modals.store.ts";
import type { IModalsStoreRegistry, ModalName } from "../../store/modals.store.ts";
import {
  CreateOrEditOrganizationDialog,
  type CreateOrEditOrganizationDialogProps,
} from "../../components/modals/create-or-edit-organization-dialog";

interface IModalsProviderRegistry extends ModalsProviderRegistryGuard<ModalName> {
  CreateOrEditOrganizationDialog: React.FC<CreateOrEditOrganizationDialogProps>;
}

const modalsRegistry: IModalsProviderRegistry = {
  CreateOrEditOrganizationDialog: CreateOrEditOrganizationDialog,
};

export const ModalsProvider = ModalsFactory.createProvider<
  ModalName,
  IModalsStoreRegistry,
  IModalsProviderRegistry
>(modalsStore, modalsRegistry);
