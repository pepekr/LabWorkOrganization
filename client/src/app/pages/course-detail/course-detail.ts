import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule, DatePipe } from '@angular/common';
import { LabTask, LabTaskService } from '../../services/LabTaskService/lab-task-service';
import { Course, CourseService } from '../../services/CourseService/course-service';
// Імпортуємо нові сервіс та компоненти
import { SubGroup, SubgroupService } from '../../services/SubgroupService/subgroup-service';
import { CreateTaskComponent } from '../../components/create-task-component/create-task-component';
import { UpdateTaskComponent } from '../../components/update-task-component/update-task-component';
import { CreateSubgroupComponent } from '../../components/create-subgroup-component/create-subgroup-component';
import { UpdateSubgroupComponent } from '../../components/update-subgroup-component/update-subgroup-component';
import { QueueComponent } from '../../components/queue/queue'; // <-- IMPORT NEW COMPONENT

@Component({
  selector: 'app-course-detail',
  templateUrl: './course-detail.html',
  styleUrls: ['./course-detail.css'],
  standalone: true,
  imports: [
    CommonModule,
    DatePipe,
    RouterLink,
    CreateTaskComponent,
    UpdateTaskComponent,
    CreateSubgroupComponent,
    UpdateSubgroupComponent,
    QueueComponent
  ]
})
export class CourseDetail implements OnInit {
  course: Course | null = null;
  tasks: LabTask[] = [];
  subgroups: SubGroup[] = [];
  loadingTasks: boolean = false;
  loadingSubgroups: boolean = false;
  error: string = '';
  courseId: string = '';

  // Модальні вікна для завдань
  showCreateTask: boolean = false;
  showUpdateTask: boolean = false;
  selectedTask: LabTask | null = null;

  // Модальні вікна для підгруп
  showCreateSubgroup: boolean = false;
  showUpdateSubgroup: boolean = false;
  selectedSubgroup: SubGroup | null = null;

  // NEW: Modal for Queue
  showQueue: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private courseService: CourseService,
    private labTaskService: LabTaskService,
    private subgroupService: SubgroupService
  ) {}

  ngOnInit(): void {
    const idFromRoute = this.route.snapshot.paramMap.get('id');
    if (!idFromRoute) {
      this.error = 'Course ID not found in URL';
      return;
    }
    this.courseId = idFromRoute;

    this.fetchCourseDetails();
    this.fetchTasks();
    this.fetchSubgroups();
  }

  fetchCourseDetails(): void {
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

  fetchSubgroups(): void {
    this.loadingSubgroups = true;
    // We MUST get the full subgroup data here, including the queue
    this.subgroupService.getAllSubgroupsByCourseId(this.courseId).subscribe({
      next: (res) => {
        this.subgroups = res;
        this.loadingSubgroups = false;
      },
      error: (err) => {
        this.error = 'Failed to load subgroups';
        console.error(err);
        this.loadingSubgroups = false;
      }
    });
  }

  // --- Subgroup Methods ---

  deleteSubgroup(subgroup: SubGroup): void {
    if (!confirm(`Are you sure you want to delete "${subgroup.name}"?`)) return;

    this.subgroupService.deleteSubgroup(subgroup.id!, this.courseId).subscribe({
      next: () => {
        this.subgroups = this.subgroups.filter(s => s.id !== subgroup.id);
      },
      error: (err) => {
        console.error('Failed to delete subgroup', err);
        alert('Failed to delete subgroup.');
      }
    });
  }

  openCreateSubgroup() {
    this.showCreateSubgroup = true;
  }

  closeCreateSubgroup(created: boolean) {
    this.showCreateSubgroup = false;
    if (created) this.fetchSubgroups();
  }

  openUpdateSubgroup(subgroup: SubGroup) {
    if (!subgroup.id) return;
    this.selectedSubgroup = subgroup;
    this.showUpdateSubgroup = true;
  }

  closeUpdateSubgroup(updated: boolean) {
    this.showUpdateSubgroup = false;
    this.selectedSubgroup = null;
    if (updated) this.fetchSubgroups(); // Re-fetch to get new student list
  }

  // --- NEW: Queue Methods ---

  openQueue(subgroup: SubGroup) {
    this.selectedSubgroup = subgroup;
    this.showQueue = true;
  }

  closeQueue() {
    this.showQueue = false;
    this.selectedSubgroup = null;
    // Re-fetch subgroups to get updated queue data
    this.fetchSubgroups();
  }

  // --- Task Methods ---

  deleteTask(task: LabTask): void {
    if (!confirm(`Are you sure you want to delete "${task.title}"?`)) return;

    this.labTaskService.deleteTask(task).subscribe({
      next: () => {
        this.tasks = this.tasks.filter(t => t.id !== task.id);
      },
      error: (err) => {
        console.error('Failed to delete task', err);
        alert('Failed to delete task.');
      }
    });
  }

  openCreateTask() {
    this.showCreateTask = true;
  }

  closeCreateTask(taskCreated: boolean) {
    this.showCreateTask = false;
    if (taskCreated) this.fetchTasks();
  }

  openUpdateTask(task: LabTask) {
    if (!task.id) return;
    this.selectedTask = task;
    this.showUpdateTask = true;
  }

  closeUpdateTask(updated: boolean) {
    this.showUpdateTask = false;
    this.selectedTask = null;
    if (updated) this.fetchTasks();
  }

  // --- NEW: Navigation Method ---

  navigateToTask(taskId?: string) {
    if (!taskId || !this.courseId) return;
    // Use the new route structure
    this.router.navigate(['/course', this.courseId, 'task', taskId]);
  }

  /**
   * Safely formats an array of DayOfWeek numbers into a string.
   */
  formatAllowedDays(days: number[]): string {
    if (!days || days.length === 0) {
      return 'None';
    }
    const dayNames = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
    return days
      .map(d => dayNames[d] || 'N/A')
      .sort((a, b) => dayNames.indexOf(a) - dayNames.indexOf(b)) // Sort them
      .join(', ');
  }
}
