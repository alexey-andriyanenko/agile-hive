import { makeAutoObservable, runInAction } from "mobx";
import { projectApiService } from "../api/project.api.ts";
import type { ProjectModel } from "../models/project.ts";
import type { CreateProjectRequest, UpdateProjectRequest } from "../api/project.types.ts";

class ProjectStore {
  private _currentProject: ProjectModel | null = null;

  private _projects: ProjectModel[] = [];

  public get currentProject(): ProjectModel | null {
    return this._currentProject;
  }

  public get projects(): ProjectModel[] {
    return this._projects;
  }

  constructor() {
    makeAutoObservable(this);
  }

  public setCurrentProject(project: ProjectModel): void {
    this._currentProject = project;
  }

  public async fetchManyProjects(organizationId: string): Promise<void> {
    const res = await projectApiService.getManyProjects(organizationId);

    runInAction(() => {
      this._projects = res.projects;
    });
  }

  public async fetchProjectById(organizationId: string, projectId: string): Promise<void> {
    const res = await projectApiService.getProjectById(organizationId, projectId);
    const index = this._projects.findIndex((p) => p.id === res.id);

    if (index !== -1) {
      runInAction(() => {
        this._projects[index] = res;
      });
    }
  }

  public async fetchCurrentProjectBySlug(
    organizationId: string,
    projectSlug: string,
  ): Promise<void> {
    const res = await projectApiService.getProjectBySlug(organizationId, projectSlug);

    runInAction(() => {
      this._currentProject = res;
    });
  }

  public async createProject(data: CreateProjectRequest): Promise<void> {
    const res = await projectApiService.createProject(data);

    runInAction(() => {
      this._projects.push(res);
    });
  }

  public async updateProject(data: UpdateProjectRequest): Promise<void> {
    const res = await projectApiService.updateProject(data);

    runInAction(() => {
      if (this._currentProject?.id === res.id) {
        this._currentProject = res;
      }

      const index = this._projects.findIndex((p) => p.id === res.id);

      if (index !== -1) {
        this._projects[index] = res;
      }
    });
  }

  public async deleteProject(organizationId: string, projectId: string): Promise<void> {
    await projectApiService.deleteProject(organizationId, projectId);

    runInAction(() => {
      this._projects = this._projects.filter((p) => p.id !== projectId);
      if (this._currentProject?.id === projectId) {
        this._currentProject = null;
      }
    });
  }
}

export const projectStore = new ProjectStore();
