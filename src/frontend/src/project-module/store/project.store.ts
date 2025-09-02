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

  public async fetchProjects(organizationId: number): Promise<void> {
    const res = await projectApiService.getProjects(organizationId);

    runInAction(() => {
      this._projects = res;
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
      const index = this._projects.findIndex((p) => p.id === res.id);

      console.log(index);

      if (index !== -1) {
        this._projects[index] = res;

        if (this._currentProject?.id === res.id) {
          this._currentProject = res;
        }
      }
    });
  }

  public async deleteProject(organizationId: number, projectId: number): Promise<void> {
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
