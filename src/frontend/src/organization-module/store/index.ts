import { organizationStore } from "./organization.store.ts";
import { organizationUserStore } from "./organization-user.store.ts";
import { modalsStore } from "./modals.store.ts";

export const useOrganizationStore = () => organizationStore;

export const useOrganizationUserStore = () => organizationUserStore;

export const useModalsStore = () => modalsStore;
