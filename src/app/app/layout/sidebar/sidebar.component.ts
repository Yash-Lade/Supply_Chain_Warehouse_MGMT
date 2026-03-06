import { Component, inject, Input } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MatListModule } from '@angular/material/list';
import { AuthService } from '../../../core/services/auth.service';

import { MatIconModule } from '@angular/material/icon';

@Component({
  standalone: true,
  selector: 'app-sidebar',
  imports: [RouterModule, MatListModule, MatIconModule],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent {
  
  @Input() collapsed = false;

  auth = inject(AuthService);

  role = this.auth.getRole();

}