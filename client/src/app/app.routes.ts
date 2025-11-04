import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { Register } from './pages/register/register';
import { Login } from './pages/login/login';
import { AuthGuard } from './services/AuthGuard/auth-guard';
import { CourseDetail } from './pages/course-detail/course-detail';
import { TaskDetail } from './pages/task-detail/task-detail';

export const routes: Routes = [
  {
    path: '',
    component: Home,
    // canActivate: [AuthGuard]
  },
  {
    path: 'course/:id', // Keep this route
    component: CourseDetail,
    // canActivate: [AuthGuard]
  },
  {
    path: 'course/:courseId/task/:id',
    component: TaskDetail,
    // canActivate: [AuthGuard]
  },
  {
    path: 'login',
    component: Login
  },
  {
    path: 'register',
    component: Register
  }
];
