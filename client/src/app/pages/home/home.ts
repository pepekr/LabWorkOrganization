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
  courses: Course[] = [];
  loading: boolean = false;
  error: string = '';

  showCreateCourse: boolean = false;
  showUpdateCourse: boolean = false;
  selectedCourse: Course | null = null;

  constructor(private courseService: CourseService, private router: Router, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.fetchCourses();
  }

  fetchCourses(): void {
    this.loading = true;
    this.error = '';
    this.courseService.getAllCourses().subscribe({
      next: (res) => {
        this.courses = res;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load courses';
        console.error(err);
        this.loading = false;
      }
    });
  }

  deleteCourse(courseId: string, useExternal: boolean = false): void {
    if (!confirm('Are you sure you want to delete this course?')) return;

    this.courseService.deleteCourse(courseId, useExternal).subscribe({
      next: () => {
        this.courses = this.courses.filter(c => c.id !== courseId);
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
    if (courseCreated) this.fetchCourses();
  }

  openUpdateCourse(course: Course) {
    // Ensure the course object has the ID
    if (!course.id) {
      console.error('Cannot update course without ID', course);
      return;
    }
    console.log(course)
    this.selectedCourse = course;
    this.showUpdateCourse = true;
  }

  closeUpdateCourse(updated: boolean) {
    this.showUpdateCourse = false;
    this.selectedCourse = null;
    if (updated) this.fetchCourses();
  }

  navigateToCourse(courseId: string) {
    if (!courseId) return;
    this.router.navigate([`${environment.apiVersion}/course`, courseId]);
  }
}
