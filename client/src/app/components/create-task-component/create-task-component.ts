import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { LabTaskCreationalDto, LabTaskService } from "../../services/LabTaskService/lab-task-service";

@Component({
  selector: 'app-create-task-component',
  templateUrl: './create-task-component.html',
  styleUrls: ['./create-task-component.css'],
  imports: [CommonModule, FormsModule],
  standalone: true
})
export class CreateTaskComponent {
  @Input() courseId!: string;
  @Output() close = new EventEmitter<boolean>();

  task: Omit<LabTaskCreationalDto, 'courseId' | 'dueDate'> = {
    title: '',
    isSentRequired: false,
    timeLimitPerStudent: '00:30:00',
    useExternal: false
  };

  dueDateString: string = this.dateToInputString(new Date());

  loading: boolean = false;
  successMessage: string = '';
  errorMessage: string = '';

  constructor(private labTaskService: LabTaskService) {}

  // --- Date helpers ---
  dateToInputString(date: Date): string {
    return new Date(date).toISOString().split('T')[0];
  }

  inputStringToDate(dateStr: string): Date {
    // Keep this simple — prevents timezone issues
    return new Date(dateStr + 'T00:00:00');
  }
  // --- ---

  submitForm(form: NgForm) {
    if (!form.valid || !this.courseId) return;

    this.loading = true;
    this.successMessage = '';
    this.errorMessage = '';

    const fullTaskDto: LabTaskCreationalDto = {
      ...this.task,
      dueDate: this.inputStringToDate(this.dueDateString),
      courseId: this.courseId
    };

    console.log('Creating task:', fullTaskDto);

    this.labTaskService.createTask(fullTaskDto).subscribe({
      next: () => {
        this.successMessage = 'Task created successfully!';
        this.loading = false;
        setTimeout(() => this.close.emit(true), 1000);
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = err.error?.message || 'Failed to create task.';
        this.loading = false;
      }
    });
  }

  cancel() {
    this.close.emit(false);
  }
}
