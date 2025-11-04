import { Component, EventEmitter, Input, Output, OnChanges, SimpleChanges } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SubgroupService, SubGroup, SubGroupStudentsDto, SubgroupUser } from "../../services/SubgroupService/subgroup-service";

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

  students: SubgroupUser[] = [];
  newEmail: string = '';

  loading: boolean = false;
  successMessage: string = '';
  errorMessage: string = '';

  constructor(private subgroupService: SubgroupService) {}

  ngOnChanges(changes: SimpleChanges) {
    if (changes['subgroup'] && this.subgroup) {
      // Клонуємо масив, щоб уникнути мутації @Input
      // this.students = [...this.subgroup.students];
      // dynamic students load
      this.loadStudents();
    }
  }

  loadStudents() {
    if (!this.subgroup?.id) return;

    this.loading = true; // Показати індикатор завантаження
    this.errorMessage = '';
    this.students = []; // Очистити список перед завантаженням

    this.subgroupService.getStudentsBySubgroupId(this.subgroup.id).subscribe({
      next: (fetchedStudents) => {
        this.students = fetchedStudents;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = err.error?.message || 'Failed to load students.';
        this.loading = false;
      }
    });
  }

  // Додавання студента до списку
  addStudent() {
    const email = this.newEmail.trim();
    if (email && email.includes('@') && !this.students.some(s => s.email === email)) {
      // Додаємо "фейкового" SubgroupUser - нам потрібен лише email
      this.students.push({ id: '', name: '', email: email });
      this.newEmail = '';
    }
  }

  // Видалення студента зі списку
  removeStudent(emailToRemove: string) {
    this.students = this.students.filter(s => s.email !== emailToRemove);
  }

  submitForm(form: NgForm) {
    if (!form.valid) return;

    this.loading = true;
    this.successMessage = '';
    this.errorMessage = '';

    // ОНОВЛЕНО: Конвертуємо наш масив студентів назад у масив email-адрес
    const studentsEmails = this.students.map(student => student.email);

    const dto: SubGroupStudentsDto = {
      subGroupId: this.subgroup.id!,
      studentsEmails: studentsEmails
    };

    // Викликаємо сервіс
    this.subgroupService.updateStudents(dto, this.subgroup.courseId).subscribe({
      next:
        () => {
          this.successMessage = 'Student list updated successfully!';
          this.loading = false;
          setTimeout(() => this.close.emit(true), 1000);
        },
      error: (err) => {
        console.error(err);
        this.errorMessage = err.error?.message
          || 'Failed to update students.';
        this.loading = false;
      }
    });
  }

  cancel() {
    this.close.emit(false);
  }
}
