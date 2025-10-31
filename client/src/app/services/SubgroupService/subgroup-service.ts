import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "../../../environments/environment.development";

// Спрощена модель юзера для безпечного відображення
export interface SubgroupUser {
  id: string;
  name: string;
  email: string;
}

// Повна модель підгрупи, яку ми очікуємо від API
export interface SubGroup {
  id?: string;
  name: string;
  allowedDays: number[]; // <-- ЗМІНЕНО
  students: SubgroupUser[];
  courseId: string;
  queue: any[]; // (QueuePlace[])
}

// DTO для СТВОРЕННЯ підгрупи
export interface SubGroupCreationalDto {
  name: string;
  allowedDays: number[]; // <-- ЗМІНЕНО
  studentsEmails: string[];
  courseId: string;
}

// DTO для ОНОВЛЕННЯ списку студентів
export interface SubGroupStudentsDto {
  subGroupId: string;
  studentsEmails: string[];
}

@Injectable({
  providedIn: 'root'
})
export class SubgroupService {
  private getBaseUrl(courseId: string) {
    // URL відповідає вашому SubGroupController.cs
    return `${environment.backendUrl}/api/courses/${courseId}/subgroups`;
  }

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
    // Цей ендпоінт ми додали у SubGroupController.cs
    const url = `${this.getBaseUrl(courseId)}/${dto.subGroupId}/students`;
    return this.http.put<SubGroup>(url, dto, {
      withCredentials: true
    });
  }

  /** Видалити підгрупу */
  deleteSubgroup(subgroupId: string, courseId: string): Observable<any> {
    // Цей ендпоінт ми додали у SubGroupController.cs
    return this.http.delete(`${this.getBaseUrl(courseId)}/${subgroupId}`, {
      withCredentials: true
    });
  }
}
