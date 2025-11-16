import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-search-task-component',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './search-task-component.html',
  styleUrls: ['./search-task-component.css']
})
export class SearchTaskComponent {
  title: string = '';
  dueDate?: string;
  useExternal: boolean = false;

  @Output() close = new EventEmitter<void>();
  @Output() search = new EventEmitter<{ title: string; dueDate?: Date; useExternal: boolean }>();

  cancel() {
    this.close.emit();
  }

  searchTasks() {
    this.search.emit({
      title: this.title.trim(),
      dueDate: this.dueDate ? new Date(this.dueDate) : undefined,
      useExternal: this.useExternal
    });
  }
}
