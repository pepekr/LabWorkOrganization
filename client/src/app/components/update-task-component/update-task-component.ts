import { Component, EventEmitter, Input, Output, OnChanges, SimpleChanges } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { LabTask, LabTaskAlterDto, LabTaskService } from "../../services/LabTaskService/lab-task-service";

@Component({
  selector: 'app-update-task-component',
  templateUrl: './update-task-component.html',
  styleUrls: ['./update-task-component.css'], // Використаємо стилі від 'update-course'
  imports: [CommonModule, FormsModule],
  standalone: true
})
export class UpdateTaskComponent implements OnChanges {
  @Input() task!: LabTask; // Отримуємо повний об'єкт завдання
  @Output() close = new EventEmitter<boolean>();

  // Створюємо копію для редагування
  updatedTask!: LabTask;
  dueDateString: string = '';

  loading: boolean = false;
  successMessage: string = '';
  errorMessage: string = '';

  constructor(private labTaskService: LabTaskService) {}

  // Цей метод спрацює, коли Angular передасть [task]
  ngOnChanges(changes: SimpleChanges) {
    if (changes['task'] && this.task) {
      // Клонуємо об'єкт, щоб не змінювати оригінал
      this.updatedTask = { ...this.task };
      this.dueDateString = this.dateToInputString(this.updatedTask.dueDate);
    }
  }

  // --- Хелпери для дати ---
  dateToInputString(date: Date): string {
    return new Date(date).toISOString().split('T')[0];
  }

  inputStringToDate(dateStr: string): Date {
    return new Date(dateStr + 'T12:00:00Z');
  }
  // --- ---

  submitForm(form: NgForm) {
    if (!form.valid) return;

    this.loading = true;
    this.successMessage = '';
    this.errorMessage = '';

    // Оновлюємо дату в нашому клонованому об'єкті
    this.updatedTask.dueDate = this.inputStringToDate(this.dueDateString);

    // Готуємо DTO для оновлення
    const alterDto: LabTaskAlterDto = {
      labTask: this.updatedTask,
      useExternal: !!this.updatedTask.externalId // Визначаємо прапор на основі наявності ID
    };

    this.labTaskService.updateTask(this.updatedTask.id!, alterDto).subscribe({
      next: () => {
        this.successMessage = 'Task updated successfully!';
        this.loading = false;
        setTimeout(() => this.close.emit(true), 1000);
      },
      error: (err) => {
        console.error(err.error);
        this.errorMessage = err.error?.message || 'Failed to update task.';
        this.loading = false;
      }
    });
  }

  cancel() {
    this.close.emit(false);
  }
}
