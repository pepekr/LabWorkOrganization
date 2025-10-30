import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { LabTaskCreationalDto, LabTaskService } from "../../services/LabTaskService/lab-task-service";

@Component({
  selector: 'app-create-task-component',
  templateUrl: './create-task-component.html',
  styleUrls: ['./create-task-component.css'], // Використаємо стилі від 'create-course'
  imports: [CommonModule, FormsModule],
  standalone: true
})
export class CreateTaskComponent {
  // Компонент повинен знати, для якого курсу він створює завдання
  @Input() courseId!: string;
  @Output() close = new EventEmitter<boolean>(); // Повідомляє батька про закриття

  // Модель форми на основі DTO
  task: Omit<LabTaskCreationalDto, 'courseId' | 'dueDate'> = {
    title: '',
    isSentRequired: false,
    timeLimitPerStudent: '00:30:00', // "HH:MM:SS"
    useExternal: false
  };

  // Використовуємо рядок для input[type=date]
  dueDateString: string = this.dateToInputString(new Date());

  loading: boolean = false;
  successMessage: string = '';
  errorMessage: string = '';

  constructor(private labTaskService: LabTaskService) {}

  // --- Хелпери для дати ---
  dateToInputString(date: Date): string {
    return new Date(date).toISOString().split('T')[0];
  }

  inputStringToDate(dateStr: string): Date {
    // Встановлюємо час за замовчуванням, щоб уникнути проблем з часовими зонами
    return new Date(dateStr + 'T12:00:00Z');
  }
  // --- ---

  submitForm(form: NgForm) {
    if (!form.valid || !this.courseId) return;

    this.loading = true;
    this.successMessage = '';
    this.errorMessage = '';

    // Збираємо повний DTO
    const fullTaskDto: LabTaskCreationalDto = {
      ...this.task,
      dueDate: this.inputStringToDate(this.dueDateString),
      courseId: this.courseId
    };

    this.labTaskService.createTask(fullTaskDto).subscribe({
      next: () => {
        this.successMessage = 'Task created successfully!';
        this.loading = false;
        // Повідомити батьківський компонент про успіх і закрити вікно
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
    this.close.emit(false); // Повідомити про скасування
  }
}
