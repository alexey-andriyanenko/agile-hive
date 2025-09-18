import React from "react";
import { ModalsFactory, type ModalsProviderRegistryGuard } from "src/modals-module";

import { modalsStore } from "../../store/modals.store.ts";
import type { IModalsStoreRegistry, ModalName } from "../../store/modals.store.ts";
import {
  CreateOrEditOrganizationDialog,
  type CreateOrEditOrganizationDialogProps,
  CreateOrEditOrganizationUserDialog,
  type CreateOrEditOrganizationUserDialogProps,
} from "../../components/modals";

interface IModalsProviderRegistry extends ModalsProviderRegistryGuard<ModalName> {
  CreateOrEditOrganizationDialog: React.FC<CreateOrEditOrganizationDialogProps>;
  CreateOrEditOrganizationUserDialog: React.FC<CreateOrEditOrganizationUserDialogProps>;
}

const modalsRegistry: IModalsProviderRegistry = {
  CreateOrEditOrganizationDialog: CreateOrEditOrganizationDialog,
  CreateOrEditOrganizationUserDialog: CreateOrEditOrganizationUserDialog,
};

export const ModalsProvider = ModalsFactory.createProvider<
  ModalName,
  IModalsStoreRegistry,
  IModalsProviderRegistry
>(modalsStore, modalsRegistry);
