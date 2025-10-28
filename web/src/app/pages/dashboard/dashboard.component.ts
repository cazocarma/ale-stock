import { Component, OnInit } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.component.html',
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
  ],
})
export class DashboardComponent implements OnInit {
  sidebarOpen = true;
  role: string | null = null;
  userName: string | null = null;

  constructor(private auth: AuthService) {}

  ngOnInit() {
    const user = this.auth.getUserInfo();
    this.role = user?.role || null;
    this.userName = user?.name || null;
  }

  toggleSidebar() {
    this.sidebarOpen = !this.sidebarOpen;
  }

  logout() {
    this.auth.logout();
  }
}
