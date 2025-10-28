import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Course, CourseAlterDto, CourseService } from "../../services/CourseService/course-service";

@Component({
  selector: 'app-update-course-component',
  templateUrl: './update-course-component.html',
  styleUrls: ['./update-course-component.css'],
  imports: [CommonModule, FormsModule],
  standalone: true
})
export class UpdateCourseComponent {
  @Input() course!: Course;
  @Output() close = new EventEmitter<boolean>();

  updatedCourse: Course = { ...this.course };
  endOfCourseString: string = this.dateToInputString(this.updatedCourse.endOfCourse);

  loading: boolean = false;
  successMessage: string = '';
  errorMessage: string = '';

  constructor(private courseService: CourseService) {}

  dateToInputString(date: Date): string {
    const d = new Date(date);
    const month = ('0' + (d.getMonth() + 1)).slice(-2);
    const day = ('0' + d.getDate()).slice(-2);
    return `${d.getFullYear()}-${month}-${day}`;
  }

  inputStringToDate(dateStr: string): Date {
    return new Date(dateStr + 'T00:00:00');
  }

  submitForm(form: NgForm) {
    if (!form.valid) return;

    this.updatedCourse.endOfCourse = this.inputStringToDate(this.endOfCourseString);

    this.loading = true;
    this.successMessage = '';
    this.errorMessage = '';

    const alterDto: CourseAlterDto = {
      course: this.updatedCourse,
      useExternal: !!this.updatedCourse.externalId
    };

    this.courseService.updateCourse(this.updatedCourse.id!, alterDto).subscribe({
      next: () => {
        this.successMessage = 'Course updated successfully!';
        this.loading = false;
        setTimeout(() => this.close.emit(true), 1000);
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Failed to update course.';
        this.loading = false;
      }
    });
  }

  cancel() {
    this.close.emit(false);
  }
}
