import { makeAutoObservable, runInAction } from "mobx";
import { type GetTagsByProjectIdRequest, tagApiService } from "../api/tag.api.ts";
import type { TagModel } from "../models/tag.ts";
import type { CreateTagRequest, UpdateTagRequest } from "../api/tag.types.ts";

class TagStore {
  private _tags: TagModel[] = [];

  constructor() {
    makeAutoObservable(this);
  }

  public get tags(): TagModel[] {
    return this._tags;
  }

  public async fetchTagsByProjectId(data: GetTagsByProjectIdRequest): Promise<void> {
    const res = await tagApiService.getManyByProjectId(data);

    runInAction(() => {
      this._tags = res.tags;
    });
  }

  public async createTag(data: CreateTagRequest): Promise<void> {
    const res = await tagApiService.create(data);

    runInAction(() => {
      this._tags.push(res);
    });
  }

  public async updateTag(data: UpdateTagRequest): Promise<void> {
    const res = await tagApiService.update(data);

    runInAction(() => {
      const index = this._tags.findIndex((t) => t.id === res.id);

      if (index !== -1) {
        this._tags[index] = res;
      }
    });
  }

  public async deleteTag(organizationId: string, tagId: string, projectId: string): Promise<void> {
    await tagApiService.delete(organizationId, tagId, projectId);

    runInAction(() => {
      this._tags = this._tags.filter((t) => t.id !== tagId);
    });
  }
}

export const tagStore = new TagStore();
