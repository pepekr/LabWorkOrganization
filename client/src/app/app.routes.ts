import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { Register } from './pages/register/register';
import { Login } from './pages/login/login';
import { AuthGuard } from './services/AuthGuard/auth-guard';

export const routes: Routes = [

    {
        path: '',
        component: Home,
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
