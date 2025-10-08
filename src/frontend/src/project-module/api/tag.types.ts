import type { TagModel } from "../models/tag.ts";

export type GetTagsResponse = {
  tags: TagModel[];
};

export type CreateTagRequest = {
  organizationId: string;
  projectId: string;
  name: string;
  color: string;
};

export type CreateTagResponse = TagModel;

export type UpdateTagRequest = {
  organizationId: string;
  tagId: string;
  projectId: string;
  name: string;
  color: string;
};

export type UpdateTagResponse = TagModel;
