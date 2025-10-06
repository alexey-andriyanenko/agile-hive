import React from "react";
import { ModalsFactory, type ModalsProviderRegistryGuard } from "src/modals-module";

import type { IModalsStoreRegistry, ModalName } from "src/board-module/store/modals.store.ts";
import {
  type CreateOrEditBoardDialogProps,
  CreateOrEditBoardDialog,
  type CreateOrEditTaskDialogProps,
  CreateOrEditTaskDialog,
} from "src/board-module/components/modals";
import { modalsStore } from "src/board-module/store/modals.store.ts";

interface IModalsProviderRegistry extends ModalsProviderRegistryGuard<ModalName> {
  CreateOrEditBoardDialog: React.FC<CreateOrEditBoardDialogProps>;
  CreateOrEditTaskDialog: React.FC<CreateOrEditTaskDialogProps>;
}

const modalsRegistry: IModalsProviderRegistry = {
  CreateOrEditBoardDialog,
  CreateOrEditTaskDialog,
};

export const ModalsProvider = ModalsFactory.createProvider<
  ModalName,
  IModalsStoreRegistry,
  IModalsProviderRegistry
>(modalsStore, modalsRegistry);
