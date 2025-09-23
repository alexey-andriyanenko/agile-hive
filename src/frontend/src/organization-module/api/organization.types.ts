import type { OrganizationModel } from "../models/organization.ts";

export type GetManyOrganizationsResponse = {
  organizations: OrganizationModel[];
};

export type CreateOrganizationRequest = {
  organizationName: string;
};

export type CreateOrganizationResponse = OrganizationModel;

export type UpdateOrganizationRequest = CreateOrganizationRequest & {
  id: string;
};

export type UpdateOrganizationResponse = OrganizationModel;
