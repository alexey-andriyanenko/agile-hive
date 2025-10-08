import { appHttpClient } from "src/shared-module/api";
import type {
  CreateTagRequest,
  CreateTagResponse,
  GetTagsResponse,
  UpdateTagRequest,
  UpdateTagResponse,
} from "./tag.types.ts";
import type { TagModel } from "../models/tag.ts";

export type GetTagsByProjectIdRequest = {
  organizationId: string;
  projectId: string;
};

type TagResponseModel = {
  id: string;
  name: string;
  color: string;
};

type GetTagsResponseModel = {
  tags: TagResponseModel[];
};

type CreateTagRequestBody = {
  name: string;
  color: string;
};

type UpdateTagRequestBody = {
  name: string;
  color: string;
};

class TagApiService {
  public async getManyByProjectId(data: GetTagsByProjectIdRequest): Promise<GetTagsResponse> {
    const response = await appHttpClient
      .get<GetTagsResponseModel>("/organizations/:organizationId/tags/by-project")
      .setRouteParams({ organizationId: data.organizationId })
      .setSearchParams({ projectId: data.projectId })
      .send();

    return {
      tags: response.tags.map(this.toTagDomainModel),
    };
  }

  public async create(data: CreateTagRequest): Promise<CreateTagResponse> {
    const requestBody: CreateTagRequestBody = {
      name: data.name,
      color: data.color,
    };

    const response = await appHttpClient
      .post<CreateTagRequestBody, TagResponseModel>("/organizations/:organizationId/tags")
      .setRouteParams({ organizationId: data.organizationId })
      .setSearchParams({ projectId: data.projectId })
      .send(requestBody);

    return this.toTagDomainModel(response);
  }

  public async update(data: UpdateTagRequest): Promise<UpdateTagResponse> {
    const requestBody: UpdateTagRequestBody = {
      name: data.name,
      color: data.color,
    };

    const response = await appHttpClient
      .put<UpdateTagRequestBody, TagResponseModel>("/organizations/:organizationId/tags/:tagId")
      .setRouteParams({ organizationId: data.organizationId, tagId: data.tagId })
      .setSearchParams({ projectId: data.projectId })
      .send(requestBody);

    return this.toTagDomainModel(response);
  }

  public async delete(organizationId: string, tagId: string, projectId: string): Promise<void> {
    await appHttpClient
      .delete<void>("/organizations/:organizationId/tags/:tagId")
      .setRouteParams({ organizationId, tagId })
      .setSearchParams({ projectId })
      .send();
  }

  // Mapping function
  private toTagDomainModel(response: TagResponseModel): TagModel {
    return {
      id: response.id,
      name: response.name,
      color: response.color,
    };
  }
}

export const tagApiService = new TagApiService();
