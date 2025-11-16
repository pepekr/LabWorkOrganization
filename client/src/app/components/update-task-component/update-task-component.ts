import {
  Component,
  EventEmitter,
  Input,
  Output,
  OnChanges,
  SimpleChanges
} from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import {
  LabTask,
  LabTaskCreationalDto, // <-- Use this DTO for update
  LabTaskService
} from '../../services/LabTaskService/lab-task-service';

@Component({
  selector: 'app-update-task-component',
  templateUrl: './update-task-component.html',
  styleUrls: ['./update-task-component.css', '../create-subgroup-component/create-subgroup-component.css'], // <-- Import subgroup styles for textarea
  imports: [CommonModule, FormsModule],
  standalone: true
})
export class UpdateTaskComponent implements OnChanges {
  @Input() task!: LabTask; // Original task from parent
  @Output() close = new EventEmitter<boolean>();

  updatedTask: LabTask = {} as LabTask; // Editable copy
  dueDateString: string = ''; // For <input type="date">
  loading = false;
  successMessage = '';
  errorMessage = '';

  constructor(private labTaskService: LabTaskService) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['task'] && this.task) {
      this.updatedTask = { ...this.task };
      this.dueDateString = this.dateToInputString(this.updatedTask.dueDate);
    }
  }

  // --- Helpers for converting dates ---
  private dateToInputString(date: any): string {
    if (!date) return '';
    const parsed = new Date(date);
    if (isNaN(parsed.getTime())) return '';
    return parsed.toISOString().split('T')[0];
  }

  private inputStringToDate(dateStr: string): Date {
    return new Date(dateStr + 'T12:00:00Z');
  }

  // --- Submit handler ---
  submitForm(form: NgForm): void {
    if (!form.valid) return;

    this.loading = true;
    this.successMessage = '';
    this.errorMessage = '';

    // Use the Creational DTO for the update payload
    const payload: LabTaskCreationalDto = {
      title: this.updatedTask.title?.trim() || '',
      description: this.updatedTask.description || '', // <-- ADDED
      dueDate: this.inputStringToDate(this.dueDateString),
      isSentRequired: !!this.updatedTask.isSentRequired,
      timeLimitPerStudent: this.normalizeTime(this.updatedTask.timeLimitPerStudent),
      courseId: this.updatedTask.courseId,
      useExternal: !!this.updatedTask.externalId
    };

    console.log('%c[UPDATE TASK] Payload sent to backend:', 'color: yellow', payload);
    console.log('%c[UPDATE TASK] Raw JSON:', 'color: orange', JSON.stringify(payload, null, 2));

    this.labTaskService.updateTask(this.updatedTask.id!, payload).subscribe({
      next: () => {
        this.successMessage = 'Task updated successfully!';
        this.loading = false;
        setTimeout(() => this.close.emit(true), 1000);
      },
      error: (err) => {
        console.error('Backend error:', err);
        this.errorMessage =
          err.error?.message ||
          err.error ||
          'Failed to update task. Please try again.';
        this.loading = false;
      }
    });
  }

  // --- Utility: normalize time string ---
  private normalizeTime(value: string | null | undefined): string {
    if (!value) return '00:00:00';
    const match = value.match(/^(\d{1,2}):(\d{1,2}):(\d{1,2})$/);
    if (match) {
      const [_, h, m, s] = match;
      return `${h.padStart(2, '0')}:${m.padStart(2, '0')}:${s.padStart(2, '0')}`;
    }
    return '00:00:00';
  }

  cancel(): void {
    this.close.emit(false);
  }
}
