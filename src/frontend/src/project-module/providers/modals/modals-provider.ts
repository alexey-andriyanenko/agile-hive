import React from "react";
import { ModalsFactory, type ModalsProviderRegistryGuard } from "src/modals-module";

import type { IModalsStoreRegistry, ModalName } from "src/project-module/store/modals.store.ts";
import {
  type CreateProjectDialogProps,
  CreateProjectDialog,
  type AddUsersToProjectDialogProps,
  AddUsersToProjectDialog,
  type CreateOrEditTagDialogProps,
  CreateOrEditTagDialog,
} from "src/project-module/components/modals";
import { modalsStore } from "src/project-module/store/modals.store.ts";

interface IModalsProviderRegistry extends ModalsProviderRegistryGuard<ModalName> {
  CreateProjectDialog: React.FC<CreateProjectDialogProps>;
  AddUsersToProjectDialog: React.FC<AddUsersToProjectDialogProps>;
  CreateOrEditTagDialog: React.FC<CreateOrEditTagDialogProps>;
}

const modalsRegistry: IModalsProviderRegistry = {
  CreateProjectDialog,
  AddUsersToProjectDialog,
  CreateOrEditTagDialog,
};

export const ModalsProvider = ModalsFactory.createProvider<
  ModalName,
  IModalsStoreRegistry,
  IModalsProviderRegistry
>(modalsStore, modalsRegistry);
