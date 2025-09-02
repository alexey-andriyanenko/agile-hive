import type { OrganizationModel } from "../models/organization.ts";

export type GetOrganizationsResponse = OrganizationModel[];

export type CreateOrganizationRequest = {
  name: string;
  description: string;
};

export type CreateOrganizationResponse = OrganizationModel;
