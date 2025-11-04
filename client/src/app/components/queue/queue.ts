import { Component, EventEmitter, Input, Output, OnChanges, SimpleChanges, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Course } from '../../services/CourseService/course-service';
import { SubGroup, QueuePlace, SubgroupService, QueuePlaceCreationalDto, SubgroupUser } from '../../services/SubgroupService/subgroup-service';
import { LabTask } from '../../services/LabTaskService/lab-task-service';
import { AuthService, AuthStatus } from '../../services/AuthService/auth-service';
import { Subscription } from 'rxjs';

interface QueueSlot extends QueuePlace {
  userEmail: string;
  userName: string;
  taskTitle: string;
  taskTimeLimit: string;
}

@Component({
  selector: 'app-queue',
  templateUrl: './queue.html',
  styleUrls: ['./queue.css'],
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class QueueComponent implements OnInit, OnChanges {
  @Input() subgroup!: SubGroup;
  @Input() course!: Course;
  @Input() tasks!: LabTask[]; // All tasks for this course
  @Output() close = new EventEmitter<void>();

  availableDates: Date[] = [];
  selectedDateString: string = '';

  queueForSelectedDate: QueueSlot[] = [];

  totalTime: string = '00:00:00';
  bookedTime: string = '00:00:00';
  remainingTime: string = '00:00:00';

  selectedTaskId: string = '';
  authStatus: AuthStatus | null = null;
  authSub: Subscription | null = null;

  loading: boolean = false;
  error: string = '';

  constructor(
    private subgroupService: SubgroupService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.authSub = this.authService.authorized$.subscribe(auth => {
      this.authStatus = auth;
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if ((changes['subgroup'] || changes['course']) && this.subgroup && this.course) {
      this.totalTime = this.course.lessonDuration;
      this.calculateAvailableDates();

      if (this.selectedDateString) {
        this.onDateSelect(this.selectedDateString);
      }
      else {
        this.calculateRemainingTime();
      }
    }
  }

  ngOnDestroy(): void {
    this.authSub?.unsubscribe();
  }

  calculateAvailableDates(): void {
    this.availableDates = [];
    const allowedDays = this.subgroup.allowedDays;
    const endDate = new Date(this.course.endOfCourse);
    let currentDate = new Date(); // Start from today
    currentDate.setHours(0, 0, 0, 0);

    while (currentDate <= endDate) {
      if (allowedDays.includes(currentDate.getDay())) {
        this.availableDates.push(new Date(currentDate));
      }
      currentDate.setDate(currentDate.getDate() + 1);
    }
  }

  onDateSelect(dateString: string): void {
    this.selectedDateString = dateString;
    this.error = '';

    if (!dateString) {
      this.queueForSelectedDate = [];
      this.calculateRemainingTime();
      return;
    }

    const selectedDate = new Date(dateString + 'T00:00:00');
    console.log(this.subgroup.queue, "check");
    const queue = this.subgroup.queue.filter(q => {
      const qDate = new Date(q.specifiedTime);
      return qDate.getFullYear() === selectedDate.getFullYear() &&
        qDate.getMonth() === selectedDate.getMonth() &&
        qDate.getDate() === selectedDate.getDate();
    });

    this.queueForSelectedDate = queue.map(q => {
      const task = this.tasks.find(t => t.id === q.taskId);
      const user = this.subgroup.students.find(s => s.id === q.userId);
      return {
        ...q,
        userEmail: user?.email || 'Unknown User',
        userName: user?.name || 'Unknown',
        taskTitle: task?.title || 'Unknown Task',
        taskTimeLimit: task?.timeLimitPerStudent || '00:00:00'
      };
    });

    this.calculateRemainingTime();
  }

  calculateRemainingTime(): void {
    let totalBookedSeconds = 0;
    for (const slot of this.queueForSelectedDate) {
      totalBookedSeconds += this.parseTimeSpanToSeconds(slot.taskTimeLimit);
    }
    this.bookedTime = this.formatSecondsToTimeSpan(totalBookedSeconds);

    const totalSeconds = this.parseTimeSpanToSeconds(this.totalTime);
    const remainingSeconds = totalSeconds - totalBookedSeconds;
    this.remainingTime = this.formatSecondsToTimeSpan(remainingSeconds);
  }

  bookSlot(): void {
    if (!this.selectedTaskId || !this.selectedDateString) {
      this.error = 'Please select a task and a date.';
      return;
    }

    const task = this.tasks.find(t => t.id === this.selectedTaskId);
    if (!task) {
      this.error = 'Selected task not found.';
      return;
    }

    const taskSeconds = this.parseTimeSpanToSeconds(task.timeLimitPerStudent);
    const remainingSeconds = this.parseTimeSpanToSeconds(this.remainingTime);

    if (taskSeconds > 0 && taskSeconds > remainingSeconds) {
      this.error = `Not enough time left (${this.remainingTime}) to book this task (${task.timeLimitPerStudent}).`;
      return;
    }

    this.loading = true;
    this.error = '';

    const dto: QueuePlaceCreationalDto = {
      subGroupId: this.subgroup.id!,
      specifiedTime: new Date(this.selectedDateString + 'T12:00:00Z').toISOString(),
      taskId: this.selectedTaskId || null
    };
    console.log(dto, "VIEW DTO");

    this.subgroupService.addToQueue(this.course.id!, this.subgroup.id!, dto).subscribe({
      next: (updatedSubgroup) => {
        console.log(updatedSubgroup, "Updated subgroup returned from backend");

        this.subgroup = updatedSubgroup;
        this.onDateSelect(this.selectedDateString);

        this.calculateRemainingTime(); // Recalculate time
        this.selectedTaskId = '';
        this.loading = false;
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to book slot.';
        this.loading = false;
      }
    });
  }

  cancelBooking(queuePlaceId: string): void {
    if (!confirm('Are you sure you want to remove this booking?')) return;

    this.loading = true;
    this.error = '';

    this.subgroupService.removeFromQueue(this.course.id!, this.subgroup.id!, queuePlaceId).subscribe({
      next: (updatedSubgroup) => {
        this.subgroup = updatedSubgroup;
        this.onDateSelect(this.selectedDateString);
        this.loading = false;
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to remove booking.';
        this.loading = false;
      }
    });
  }

  // --- TimeSpan Helper Functions ---
  parseTimeSpanToSeconds(time: string): number {
    try {
      const parts = time.split(':');
      const h = parseInt(parts[0], 10) || 0;
      const m = parseInt(parts[1], 10) || 0;
      const s = parseInt(parts[2], 10) || 0;
      return (h * 3600) + (m * 60) + s;
    } catch {
      return 0;
    }
  }

  formatSecondsToTimeSpan(totalSeconds: number): string {
    if (totalSeconds < 0) totalSeconds = 0;
    const h = Math.floor(totalSeconds / 3600).toString().padStart(2, '0');
    const m = Math.floor((totalSeconds % 3600) / 60).toString().padStart(2, '0');
    const s = (totalSeconds % 60).toString().padStart(2, '0');
    return `${h}:${m}:${s}`;
  }
}
