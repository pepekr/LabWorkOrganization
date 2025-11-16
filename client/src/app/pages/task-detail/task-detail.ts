import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router'; // <--- Ensure RouterLink is imported
import { CommonModule, DatePipe } from '@angular/common'; // <--- Ensure DatePipe is imported
import { LabTask, LabTaskService } from '../../services/LabTaskService/lab-task-service';
import { UserTask, UserTaskService } from '../../services/UserTaskService/user-task-service';
import { Observable, of } from 'rxjs';
import { catchError, switchMap, tap } from 'rxjs/operators';

@Component({
  selector: 'app-task-detail',
  templateUrl: './task-detail.html',
  styleUrls: ['./task-detail.css'],
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe]
})
export class TaskDetail implements OnInit {
  task: LabTask | null = null;
  userTask: UserTask | null = null;
  loading: boolean = true;
  error: string = '';
  taskId: string = '';
  courseId: string = '';

  constructor(
    private route: ActivatedRoute,
    private labTaskService: LabTaskService,
    private userTaskService: UserTaskService
  ) {}

  ngOnInit(): void {
    const taskId = this.route.snapshot.paramMap.get('id');
    const courseId = this.route.snapshot.paramMap.get('courseId');

    if (!taskId || !courseId) {
      this.error = 'Task or Course ID not found in URL.';
      this.loading = false;
      return;
    }

    this.taskId = taskId;
    this.courseId = courseId;
    this.loadData();
  }

  loadData(): void {
    this.loading = true;
    this.error = '';

    // We can now use the real courseId for both the service call and the "Back" button
    this.labTaskService.getTaskById(this.taskId, this.courseId, false).pipe(
      tap(task => {
        this.task = task;
      }),
      switchMap(task => {
        if (!task) throw new Error('Task not found');
        return this.userTaskService.getUserTaskStatus(this.taskId).pipe(
          catchError(() => {
            return of({ id: '', userId: '', taskId: this.taskId, isCompleted: false });
          })
        );
      })
    ).subscribe({
      next: (userTask) => {
        this.userTask = userTask;
        this.loading = false;
      },
      error: (err) => {
        this.error = err.message || 'Failed to load task details.';
        this.loading = false;
      }
    });
  }

  toggleCompletion(): void {
    if (!this.userTask || this.loading) return;

    this.loading = true;
    const action = this.userTask.isCompleted
      ? this.userTaskService.markAsReturned(this.taskId)
      : this.userTaskService.markAsCompleted(this.taskId);

    action.subscribe({
      next: (updatedUserTask) => {
        this.userTask = updatedUserTask;
        this.loading = false;
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to update status.';
        this.loading = false;
      }
    });
  }
}
