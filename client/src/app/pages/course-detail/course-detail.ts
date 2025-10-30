import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule, DatePipe } from '@angular/common';
import { LabTask, LabTaskService } from '../../services/LabTaskService/lab-task-service';
import { Course, CourseService } from '../../services/CourseService/course-service';
import { CreateTaskComponent } from '../../components/create-task-component/create-task-component';
import { UpdateTaskComponent } from '../../components/update-task-component/update-task-component';

@Component({
  selector: 'app-course-detail',
  templateUrl: './course-detail.html',
  styleUrls: ['./course-detail.css'], // Використаємо стилі від 'home'
  standalone: true,
  imports: [CommonModule, DatePipe, RouterLink, CreateTaskComponent, UpdateTaskComponent]
})
export class CourseDetail implements OnInit {
  course: Course | null = null;
  tasks: LabTask[] = [];
  loadingTasks: boolean = false;
  error: string = '';
  courseId: string = ''; // ID курсу з URL

  // Стани для модальних вікон
  showCreateTask: boolean = false;
  showUpdateTask: boolean = false;
  selectedTask: LabTask | null = null;

  constructor(
    private route: ActivatedRoute, // Щоб читати ID з URL
    private router: Router,
    private courseService: CourseService,
    private labTaskService: LabTaskService
  ) {}

  ngOnInit(): void {
    // Отримуємо 'id' з URL (як налаштуємо в роутері)
    const idFromRoute = this.route.snapshot.paramMap.get('id');
    if (!idFromRoute) {
      this.error = 'Course ID not found in URL';
      return;
    }
    this.courseId = idFromRoute;

    // Завантажуємо деталі курсу ТА його завдання
    this.fetchCourseDetails();
    this.fetchTasks();
  }

  fetchCourseDetails(): void {
    // TODO: Потрібна логіка для визначення, чи є курс external (зараз false)
    this.courseService.getCourseById(this.courseId, false).subscribe({
      next: (res) => this.course = res,
      error: (err) => {
        this.error = 'Failed to load course details';
        console.error(err);
      }
    });
  }

  fetchTasks(): void {
    this.loadingTasks = true;
    this.error = '';
    //
    this.labTaskService.getAllTasksByCourseId(this.courseId).subscribe({
      next: (res) => {
        this.tasks = res;
        this.loadingTasks = false;
      },
      error: (err) => {
        this.error = 'Failed to load tasks';
        console.error(err);
        this.loadingTasks = false;
      }
    });
  }

  deleteTask(task: LabTask): void {
    if (!confirm(`Are you sure you want to delete "${task.title}"?`)) return;

    this.labTaskService.deleteTask(task).subscribe({ //
      next: () => {
        // Оновлюємо список, видаливши завдання
        this.tasks = this.tasks.filter(t => t.id !== task.id);
      },
      error: (err) => {
        console.error('Failed to delete task', err);
        alert('Failed to delete task.');
      }
    });
  }

  // --- Управління модальними вікнами (як у home.ts) ---

  openCreateTask() {
    this.showCreateTask = true;
  }

  closeCreateTask(taskCreated: boolean) {
    this.showCreateTask = false;
    if (taskCreated) this.fetchTasks(); // Оновлюємо список завдань
  }

  openUpdateTask(task: LabTask) {
    if (!task.id) return;
    this.selectedTask = task; // Передаємо обране завдання у модальне вікно
    this.showUpdateTask = true;
  }

  closeUpdateTask(updated: boolean) {
    this.showUpdateTask = false;
    this.selectedTask = null;
    if (updated) this.fetchTasks(); // Оновлюємо список завдань
  }
}
