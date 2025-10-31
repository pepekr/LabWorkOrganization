import { Component, EventEmitter, Input, Output, OnChanges, SimpleChanges } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SubgroupService, SubGroup, SubGroupStudentsDto } from "../../services/SubgroupService/subgroup-service";

@Component({
  selector: 'app-update-subgroup-component',
  templateUrl: './update-subgroup-component.html',
  styleUrls: ['./update-subgroup-component.css'],
  imports: [CommonModule, FormsModule],
  standalone: true
})
export class UpdateSubgroupComponent implements OnChanges {
  @Input() subgroup!: SubGroup;
  @Output() close = new EventEmitter<boolean>();

  studentsEmailsRaw: string = ''; // Для <textarea>
  loading: boolean = false;
  successMessage: string = '';
  errorMessage: string = '';

  constructor(private subgroupService: SubgroupService) {}

  // Цей метод заповнить форму, коли компонент отримає дані
  ngOnChanges(changes: SimpleChanges) {
    if (changes['subgroup'] && this.subgroup) {
      // Заповнюємо textarea email-адресами студентів, які вже є в підгрупі
      this.studentsEmailsRaw = this.subgroup.students
        .map(s => s.email) // Беремо email з SubgroupUser
        .join('\n'); // Кожен email на новому рядку
    }
  }

  submitForm(form: NgForm) {
    if (!form.valid) return;

    this.loading = true;
    this.successMessage = '';
    this.errorMessage = '';

    // Конвертуємо textarea назад в масив email-адрес
    const studentsEmails = this.studentsEmailsRaw
      .split(/[\n,;]+/)
      .map(email => email.trim())
      .filter(email => email.length > 0 && email.includes('@'));

    const dto: SubGroupStudentsDto = {
      subGroupId: this.subgroup.id!,
      studentsEmails: studentsEmails
    };

    // Викликаємо сервіс
    this.subgroupService.updateStudents(dto, this.subgroup.courseId).subscribe({
      next: () => {
        this.successMessage = 'Student list updated successfully!';
        this.loading = false;
        setTimeout(() => this.close.emit(true), 1000);
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = err.error?.message || 'Failed to update students.';
        this.loading = false;
      }
    });
  }

  cancel() {
    this.close.emit(false);
  }
}
