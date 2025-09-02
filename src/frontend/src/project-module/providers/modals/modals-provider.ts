import React from "react";
import { ModalsFactory, type ModalsProviderRegistryGuard } from "src/modals-module";

import type { IModalsStoreRegistry, ModalName } from "src/project-module/store/modals.store.ts";
import {
  type CreateOrEditProjectDialogProps,
  CreateOrEditProjectDialog,
} from "src/project-module/components/modals/create-or-edit-project-dialog";
import { modalsStore } from "src/project-module/store";

interface IModalsProviderRegistry extends ModalsProviderRegistryGuard<ModalName> {
  CreateOrEditProjectDialog: React.FC<CreateOrEditProjectDialogProps>;
}

const modalsRegistry: IModalsProviderRegistry = {
  CreateOrEditProjectDialog,
};

export const ModalsProvider = ModalsFactory.createProvider<
  ModalName,
  IModalsStoreRegistry,
  IModalsProviderRegistry
>(modalsStore, modalsRegistry);
