import { Component, EventEmitter, Input, Output, OnChanges, SimpleChanges } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Course, CourseAlterDto, CourseService } from "../../services/CourseService/course-service";
import { ActivatedRoute } from '@angular/router';
import { environment } from "../../../environments/environment.development"

@Component({
  selector: 'app-update-course-component',
  templateUrl: './update-course-component.html',
  styleUrls: ['./update-course-component.css'],
  imports: [CommonModule, FormsModule],
  standalone: true
})
export class UpdateCourseComponent implements OnChanges {
  @Input() course!: Course;
  @Output() close = new EventEmitter<boolean>();
  apiVersion = environment.apiVersion;
  updatedCourse!: Course;
  endOfCourseString: string = '';
  loading: boolean = false;
  successMessage: string = '';
  errorMessage: string = '';

  constructor(private courseService: CourseService, private route: ActivatedRoute) {}

  // Called whenever @Input() changes
  ngOnChanges(changes: SimpleChanges) {
    if (changes['course'] && this.course) {
      this.updatedCourse = { ...this.course };
      this.endOfCourseString = this.dateToInputString(this.updatedCourse.endOfCourse);
    }
  }

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
      course: {
        id: this.updatedCourse.id,
        name: this.updatedCourse.name,
        lessonDuration: this.updatedCourse.lessonDuration,
        endOfCourse: this.updatedCourse.endOfCourse,
        externalId: this.updatedCourse.externalId,
        ownerId: this.updatedCourse.ownerId,
        description: this.apiVersion === 'v2' ? this.updatedCourse.description : ""
      },
      useExternal: !!this.updatedCourse.externalId
    };

    console.log("Course to update", alterDto.course);

    this.courseService.updateCourse(this.updatedCourse.id!, alterDto).subscribe({
      next: () => {
        this.successMessage = 'Course updated successfully!';
        this.loading = false;
        setTimeout(() => this.close.emit(true), 1000);
      },
      error: (err) => {
        console.error(err.error);
        this.errorMessage = 'Failed to update course.';
        this.loading = false;
      }
    });
  }

  cancel() {
    this.close.emit(false);
  }
}
