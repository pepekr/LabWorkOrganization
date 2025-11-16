import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "../../../environments/environment.development";

export interface Course {
  id?: string;
  externalId?: string;
  ownerId?: string;
  name: string;
  description?: string;
  lessonDuration: string; // ISO string for TimeSpan equivalent
  endOfCourse: Date;
}

export interface CourseCreationalDto {
  name: string;
  lessonDuration: string; // e.g. "01:30:00"
  endOfCourse: Date;
  useExternal: boolean;
}

export interface CourseCreationalDtoV2 extends CourseCreationalDto {
  description?: string;
}

export interface CourseAlterDto {
  course: Course;
  useExternal: boolean;
}


@Injectable({
  providedIn: 'root'
})
export class CourseService {
  private readonly baseUrl = `${environment.backendUrl}/api/${environment.apiVersion}/courses`;
  private readonly userBaseUrl = `${environment.backendUrl}/api/users`;

  constructor(private http: HttpClient) {}

  /** Get all courses for current user */
  getAllCourses(isGetExternal: boolean = false): Observable<Course[]> {
    const params = new HttpParams().set('isGetExternal', isGetExternal);
    return this.http.get<Course[]>(`${this.baseUrl}/getAllByUserId`, { params, withCredentials:true });
  }

  /** Get a specific course by ID */
  getCourseById(id: string, isExternalCourse: boolean): Observable<Course> {
    const params = new HttpParams().set('isExternalCourse', isExternalCourse);
    return this.http.get<Course>(`${this.baseUrl}/getById/${id}`, { params, withCredentials:true });
  }

  /** Create a new course */
  createCourse(course: CourseCreationalDtoV2): Observable<Course> {
    if (environment.apiVersion === 'v1') {
      const redactedCourse = {
        name: course.name,
        lessonDuration: course.lessonDuration,
        endOfCourse: course.endOfCourse,
        useExternal: course.useExternal
      }
      return this.http.post<Course>(`${this.baseUrl}/create`, redactedCourse,{ withCredentials:true});
    }
    return this.http.post<Course>(`${this.baseUrl}/create`, course,{ withCredentials:true});
  }

  /** Update existing course */
  updateCourse(id: string, courseAlter: CourseAlterDto): Observable<Course> {
    return this.http.patch<Course>(`${this.baseUrl}/update/${id}`, courseAlter, {withCredentials:true});
  }

  /** Delete course */
  deleteCourse(id: string, isExternalCourse: boolean): Observable<any> {
    // Backend expects isExternalCourse in body, not query
    return this.http.request('delete', `${this.baseUrl}/delete/${id}`, {
      body: isExternalCourse,withCredentials:true
    });
  }

  getStudentCourses(): Observable<Course[]> {
    return this.http.get<Course[]>(`${this.userBaseUrl}/student/courses`, { withCredentials: true });
  }
}
