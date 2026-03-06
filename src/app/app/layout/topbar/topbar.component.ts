import { Component, EventEmitter, Output } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-topbar',
  imports: [MatToolbarModule, MatIconModule, MatButtonModule],
  templateUrl: './topbar.component.html',
  styleUrls: ['./topbar.component.scss']
})
export class TopbarComponent {

  @Output() toggleSidebar = new EventEmitter();

  constructor(private router:Router){}

  logout(){

    localStorage.removeItem('token');

    this.router.navigate(['/login']);

  }

}