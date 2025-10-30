import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "../../../environments/environment.development";

export interface LabTask {
  id?: string;
  externalId?: string;
  title: string;
  dueDate: Date;
  isSentRequired: boolean;
  timeLimitPerStudent: string; // ISO string for TimeSpan
  courseId: string;
}

export interface LabTaskCreationalDto {
  title: string;
  dueDate: Date;
  isSentRequired: boolean;
  timeLimitPerStudent: string; // "HH:MM:SS"
  courseId: string;
  useExternal: boolean;
}

export interface LabTaskAlterDto {
  labTask: LabTask;
  useExternal: boolean;
}

export interface LabTaskGetDto {
  courseId: string;
  useExternal: boolean;
}
@Injectable({
  providedIn: 'root'
})
export class LabTaskService {
  private getBaseUrl(courseId: string) {
    return `${environment.backendUrl}/api/courses/${courseId}/tasks`;
  }

  constructor(private http: HttpClient) {}

  getAllTasksByCourseId(courseId: string): Observable<LabTask[]> {
    return this.http.get<LabTask[]>(`${this.getBaseUrl(courseId)}/getAll`, {
      withCredentials: true
    });
  }

  getTaskById(id: string, courseId: string, useExternal: boolean): Observable<LabTask> {
    const getDto: LabTaskGetDto = { courseId, useExternal };
    return this.http.request<LabTask>('get', `${this.getBaseUrl(courseId)}/getById/${id}`, {
      body: getDto,
      withCredentials: true
    });
  }

  createTask(task: LabTaskCreationalDto): Observable<LabTask> {
    return this.http.post<LabTask>(`${this.getBaseUrl(task.courseId)}/create`, task, {
      withCredentials: true
    });
  }

  updateTask(id: string, taskAlter: LabTaskAlterDto): Observable<LabTask> {
    return this.http.patch<LabTask>(`${this.getBaseUrl(taskAlter.labTask.courseId)}/update/${id}`, taskAlter, {
      withCredentials: true
    });
  }

  deleteTask(task: LabTask): Observable<any> {
    const alterDto: LabTaskAlterDto = {
      labTask: task,
      useExternal: !!task.externalId
    };

    return this.http.request('delete', `${this.getBaseUrl(task.courseId)}/delete/${task.id}`, {
      body: alterDto,
      withCredentials: true
    });
  }

}
