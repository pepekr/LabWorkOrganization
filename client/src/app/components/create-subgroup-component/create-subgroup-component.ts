import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SubgroupService, SubGroupCreationalDto } from "../../services/SubgroupService/subgroup-service";

// Новий допоміжний інтерфейс
interface DayOption {
  name: string;
  value: number; // 0 = Sunday, 1 = Monday, etc. (згідно .NET DayOfWeek Enum)
  checked: boolean;
}

@Component({
  selector: 'app-create-subgroup-component',
  templateUrl: './create-subgroup-component.html',
  styleUrls: ['./create-subgroup-component.css'],
  imports: [CommonModule, FormsModule],
  standalone: true
})
export class CreateSubgroupComponent {
  @Input() courseId!: string;
  @Output() close = new EventEmitter<boolean>();

  // DTO для форми
  dto = {
    name: '',
    studentsEmailsRaw: '' // Використовуємо для <textarea>
  };

  // Модель для чекбоксів днів тижня (ОНОВЛЕНО)
  daysOfWeek: DayOption[] = [
    { name: 'Sunday',    value: 0, checked: false },
    { name: 'Monday',    value: 1, checked: true },
    { name: 'Tuesday',   value: 2, checked: true },
    { name: 'Wednesday', value: 3, checked: true },
    { name: 'Thursday',  value: 4, checked: true },
    { name: 'Friday',    value: 5, checked: true },
    { name: 'Saturday',  value: 6, checked: false }
  ];

  loading: boolean = false;
  successMessage: string = '';
  errorMessage: string = '';

  constructor(private subgroupService: SubgroupService) {}

  // 'get dayKeys()' більше не потрібен

  submitForm(form: NgForm) {
    if (!form.valid || !this.courseId) return;

    this.loading = true;
    this.successMessage = '';
    this.errorMessage = '';

    // 1. Конвертуємо масив об'єктів днів у масив чисел (ОНОВЛЕНО)
    const allowedDays = this.daysOfWeek
      .filter(day => day.checked) // Беремо тільки обрані
      .map(day => day.value);     // Витягуємо їх числові значення

    // 2. Конвертуємо textarea в масив email-адрес
    const studentsEmails = this.dto.studentsEmailsRaw
      .split(/[\n,;]+/) // Розділяємо по нових рядках, комах або крапках з комою
      .map(email => email.trim()) // Обрізаємо пробіли
      .filter(email => email.length > 0 && email.includes('@')); // Беремо лише валідні рядки

    const fullDto: SubGroupCreationalDto = {
      name: this.dto.name,
      courseId: this.courseId,
      allowedDays: allowedDays, // Тепер це number[]
      studentsEmails: studentsEmails
    };

    // Викликаємо сервіс
    this.subgroupService.createSubgroup(fullDto).subscribe({
      next: () => {
        this.successMessage = 'Subgroup created successfully!';
        this.loading = false;
        // Закриваємо модальне вікно через 1 секунду і сповіщаємо батька
        setTimeout(() => this.close.emit(true), 1000);
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = err.error?.message || 'Failed to create subgroup.';
        this.loading = false;
      }
    });
  }

  cancel() {
    this.close.emit(false); // Сповіщаємо батька, що вікно закрите без змін
  }
}
