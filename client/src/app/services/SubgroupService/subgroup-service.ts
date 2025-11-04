import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "../../../environments/environment.development";
import { LabTask } from '../LabTaskService/lab-task-service';

// Спрощена модель юзера для безпечного відображення
export interface SubgroupUser {
  id: string;
  name: string;
  email: string;
}

// DTO для черги, яке ми очікуємо (з вашим TaskId)
export interface QueuePlace {
  id: string;
  userId: string;
  subGroupId: string;
  specifiedTime: Date; // This will be the date of the class
  taskId: string;

  // Optional: For frontend display
  user?: SubgroupUser;
  task?: LabTask;
}

// Повна модель підгрупи, яку ми очікуємо від API
export interface SubGroup {
  id?: string;
  name: string;
  allowedDays: number[];
  students: SubgroupUser[];
  courseId: string;
  queue: QueuePlace[]; // <-- MODIFIED from any[]
}

// DTO для СТВОРЕННЯ підгрупи
export interface SubGroupCreationalDto {
  name: string;
  allowedDays: number[];
  studentsEmails: string[];
  courseId: string;
}

// DTO для ОНОВЛЕННЯ списку студентів
export interface SubGroupStudentsDto {
  subGroupId: string;
  studentsEmails: string[];
}

// DTO для додавання в чергу (з вашим TaskId)
export interface QueuePlaceCreationalDto {
  subGroupId: string;
  specifiedTime: string;
  taskId: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class SubgroupService {
  private getBaseUrl(courseId: string) {
    return `${environment.backendUrl}/api/courses/${courseId}/subgroups`;
  }
  private userApiUrl = `${environment.backendUrl}/api/users`;

  constructor(private http: HttpClient) {}

  /** Отримати всі підгрупи для конкретного курсу */
  getAllSubgroupsByCourseId(courseId: string): Observable<SubGroup[]> {
    return this.http.get<SubGroup[]>(`${this.getBaseUrl(courseId)}/getAll`, {
      withCredentials: true
    });
  }

  /** Створити нову підгрупу */
  createSubgroup(dto: SubGroupCreationalDto): Observable<SubGroup> {
    return this.http.post<SubGroup>(`${this.getBaseUrl(dto.courseId)}/create`, dto, {
      withCredentials: true
    });
  }

  /** Оновити список студентів у підгрупі */
  updateStudents(dto: SubGroupStudentsDto, courseId: string): Observable<SubGroup> {
    const url = `${this.getBaseUrl(courseId)}/${dto.subGroupId}/students`;
    return this.http.put<SubGroup>(url, dto, {
      withCredentials: true
    });
  }

  /** Видалити підгрупу */
  deleteSubgroup(subgroupId: string, courseId: string): Observable<any> {
    return this.http.delete(`${this.getBaseUrl(courseId)}/${subgroupId}`, {
      withCredentials: true
    });
  }

  getStudentsBySubgroupId(subgroupId: string): Observable<SubgroupUser[]> {
    return this.http.get<SubgroupUser[]>(`${this.userApiUrl}/subgroup/${subgroupId}`, {
      withCredentials: true
    });
  }

  // --- NEW QUEUE METHODS ---

  /** Додати поточного юзера в чергу */
  addToQueue(courseId: string, subGroupId: string, dto: QueuePlaceCreationalDto) {
    return this.http.post<SubGroup>(
      `${this.getBaseUrl(courseId)}/subgroups/${subGroupId}/queue/add`,
      dto
    );
  }



  /** Видалити запис з черги */
  removeFromQueue(courseId: string, subGroupId: string, queuePlaceId: string): Observable<any> {
    const url = `${this.getBaseUrl(courseId)}/${subGroupId}/queue/remove`;
    // Backend expects the ID in the body as a raw string
    return this.http.post(url, `"${queuePlaceId}"`, {
      headers: { 'Content-Type': 'application/json' },
      withCredentials: true
    });
  }
}
