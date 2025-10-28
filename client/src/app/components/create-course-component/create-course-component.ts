import { Component, EventEmitter, Output } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CourseCreationalDto, CourseService } from "../../services/CourseService/course-service";

@Component({
  selector: 'app-create-course-component',
  templateUrl: './create-course-component.html',
  styleUrls: ['./create-course-component.css'],
  imports: [CommonModule, FormsModule],
  standalone: true
})
export class CreateCourseComponent {
  @Output() close = new EventEmitter<boolean>(); // emits true when closing

  course: CourseCreationalDto = {
    name: '',
    lessonDuration: '01:00:00',
    endOfCourse: new Date(),
    useExternal: false
  };

  endOfCourseString: string = this.dateToInputString(this.course.endOfCourse);

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

    this.course.endOfCourse = this.inputStringToDate(this.endOfCourseString);

    this.loading = true;
    this.successMessage = '';
    this.errorMessage = '';

    this.courseService.createCourse(this.course).subscribe({
      next: () => {
        this.successMessage = 'Course created successfully!';
        form.resetForm();
        this.course = {
          name: '',
          lessonDuration: '01:00:00',
          endOfCourse: new Date(),
          useExternal: false
        };
        this.endOfCourseString = this.dateToInputString(this.course.endOfCourse);
        this.loading = false;

        // Notify parent that modal should close
        setTimeout(()=>{this.close.emit(true);}, 1000)
        
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Failed to create course.';
        this.loading = false;
      }
    });
  }

  cancel() {
    this.close.emit(false); // emit true when user clicks the close button
  }
}
