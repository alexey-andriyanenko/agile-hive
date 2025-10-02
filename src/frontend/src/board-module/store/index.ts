import { boardStore } from "./board.store.ts";
import { taskStore } from "./task.store.ts";
import { modalsStore } from "./modals.store.ts";

export const useBoardStore = () => boardStore;
export const useTaskStore = () => taskStore;

export const useModalsStore = () => modalsStore;
