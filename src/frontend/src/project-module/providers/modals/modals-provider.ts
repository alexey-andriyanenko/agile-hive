import React from "react";
import { ModalsFactory, type ModalsProviderRegistryGuard } from "src/modals-module";

import type { IModalsStoreRegistry, ModalName } from "src/project-module/store/modals.store.ts";
import {
  type CreateOrEditProjectDialogProps,
  CreateOrEditProjectDialog,
  type AddUsersToProjectDialogProps,
  AddUsersToProjectDialog,
} from "src/project-module/components/modals";
import { modalsStore } from "src/project-module/store/modals.store.ts";

interface IModalsProviderRegistry extends ModalsProviderRegistryGuard<ModalName> {
  CreateOrEditProjectDialog: React.FC<CreateOrEditProjectDialogProps>;
  AddUsersToProjectDialog: React.FC<AddUsersToProjectDialogProps>;
}

const modalsRegistry: IModalsProviderRegistry = {
  CreateOrEditProjectDialog,
  AddUsersToProjectDialog,
};

export const ModalsProvider = ModalsFactory.createProvider<
  ModalName,
  IModalsStoreRegistry,
  IModalsProviderRegistry
>(modalsStore, modalsRegistry);
