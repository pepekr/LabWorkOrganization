import { Component, OnInit } from '@angular/core';
import { Course, CourseService } from "../../services/CourseService/course-service";
import { CreateCourseComponent } from "../../components/create-course-component/create-course-component";
import { UpdateCourseComponent } from "../../components/update-course-component/update-course-component";
import { CommonModule, DatePipe } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { environment } from "../../../environments/environment.development";

@Component({
  selector: 'app-home',
  templateUrl: './home.html',
  styleUrls: ['./home.css'],
  standalone: true,
  imports: [CreateCourseComponent, UpdateCourseComponent, DatePipe, CommonModule]
})
export class Home implements OnInit {
  studentCourses: Course[] = [];
  ownedCourses: Course[] = [];
  loadingStudent: boolean = false;
  errorStudent: string = '';
  loadingOwned: boolean = false;
  errorOwned: string = '';

  showCreateCourse: boolean = false;
  showUpdateCourse: boolean = false;
  selectedCourse: Course | null = null;

  constructor(private courseService: CourseService, private router: Router, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.fetchStudentCourses();
    this.fetchOwnedCourses();
  }

  fetchStudentCourses(): void {
    this.loadingStudent = true;
    this.errorStudent = '';
    // This calls GET /api/users/student/courses
    this.courseService.getStudentCourses().subscribe({
      next: (res) => {
        this.studentCourses = res;
        this.loadingStudent = false;
      },
      error: (err) => {
        this.errorStudent = 'Failed to load your enrolled courses.';
        console.error(err);
        this.loadingStudent = false;
      }
    });
  }

  fetchOwnedCourses(): void {
    this.loadingOwned = true;
    this.errorOwned = '';
    // This calls GET /api/courses/getAllByUserId
    this.courseService.getAllCourses().subscribe({
      next: (res) => {
        this.ownedCourses = res;
        this.loadingOwned = false;
      },
      error: (err) => {
        this.errorOwned = 'Failed to load your owned courses.';
        console.error(err);
        this.loadingOwned = false;
      }
    });
  }

  deleteCourse(courseId: string, useExternal: boolean = false): void {
    if (!confirm('Are you sure you want to delete this course?')) return;

    this.courseService.deleteCourse(courseId, useExternal).subscribe({
      next: () => {
        // Remove from the owned list only
        this.ownedCourses = this.ownedCourses.filter(c => c.id !== courseId);
      },
      error: (err) => {
        console.error('Failed to delete course', err);
        alert('Failed to delete course.');
      }
    });
  }

  openCreateCourse() {
    this.showCreateCourse = true;
  }

  closeCreateCourse(courseCreated: boolean) {
    this.showCreateCourse = false;
    if (courseCreated) {
      this.fetchOwnedCourses(); // Refresh owned list
    }
  }

  openUpdateCourse(course: Course) {
    if (!course.id) return;
    this.selectedCourse = course;
    this.showUpdateCourse = true;
  }

  closeUpdateCourse(updated: boolean) {
    this.showUpdateCourse = false;
    this.selectedCourse = null;
    if (updated) {
      this.fetchOwnedCourses(); // Refresh owned list
    }
  }

  navigateToCourse(courseId: string) {
    if (!courseId) return;
    this.router.navigate([`${environment.apiVersion}/course`, courseId]);
  }
}
