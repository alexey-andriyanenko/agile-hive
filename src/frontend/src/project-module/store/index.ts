import { projectStore } from "./project.store.ts";
import { modalsStore } from "./modals.store.ts";
import { projectUserStore } from "./project-user.store.ts";

export const useProjectStore = () => projectStore;

export const useProjectUserStore = () => projectUserStore;

export const useModalsStore = () => modalsStore;
