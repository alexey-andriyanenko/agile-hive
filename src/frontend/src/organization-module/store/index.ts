import { organizationStore } from "./organization.store.ts";
import { modalsStore } from "./modals.store.ts";

export const useOrganizationStore = () => organizationStore;
export const useModalsStore = () => modalsStore;
