import { projectStore } from "./project.store.ts";
import { modalsStore } from "./modals.store.ts";

export const useProjectStore = () => projectStore;
export const useModalsStore = () => modalsStore;
