import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "../../../environments/environment.development";

// Based on server\LabWorkOrganization.Domain\Entities\UserTask.cs
export interface UserTask {
  id: string;
  userId: string;
  taskId: string;
  isCompleted: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class UserTaskService {
  private getBaseUrl(taskId: string) {
    return `${environment.backendUrl}/api/tasks/${taskId}`;
  }

  constructor(private http: HttpClient) {}

  /**
   * Gets the completion status for the current user for a specific task.
   */
  getUserTaskStatus(taskId: string): Observable<UserTask> {
    return this.http.get<UserTask>(`${this.getBaseUrl(taskId)}/my-status`, {
      withCredentials: true
    });
  }

  /**
   * Marks a task as completed for the current user.
   */
  markAsCompleted(taskId: string): Observable<UserTask> {
    return this.http.post<UserTask>(`${this.getBaseUrl(taskId)}/complete`, {}, {
      withCredentials: true
    });
  }

  /**
   * Marks a task as "returned" (not completed) for the current user.
   */
  markAsReturned(taskId: string): Observable<UserTask> {
    return this.http.post<UserTask>(`${this.getBaseUrl(taskId)}/return`, {}, {
      withCredentials: true
    });
  }
}
