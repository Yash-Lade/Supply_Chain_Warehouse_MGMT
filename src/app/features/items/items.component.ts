import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ItemsService, Item } from '../../core/services/items.service';

@Component({
  standalone: true,
  selector: 'app-items',
  imports: [CommonModule],
  templateUrl: './items.component.html'
})
export class ItemsComponent implements OnInit {

  private itemsService = inject(ItemsService);

  items: Item[] = [];
  loading = false;

  ngOnInit(): void {
    this.loadItems();
  }

  loadItems() {
    this.loading = true;

    this.itemsService.getAll().subscribe({
      next: data => {
        this.items = data;
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }
}