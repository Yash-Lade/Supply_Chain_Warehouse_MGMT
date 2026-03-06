import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { MainLayoutComponent } from './app/layout/main-layout/main-layout.component';
import { ItemsComponent } from './features/items/items.component';
import { WarehousesComponent } from './features/warehouses/warehouses.component';
export const routes: Routes = [

  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component')
        .then(m => m.LoginComponent)
  },

  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register.component')
        .then(m => m.RegisterComponent)
  },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [AuthGuard],
    children: [

      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard.component')
            .then(m => m.DashboardComponent)
      },

      {
        path: 'purchase-orders',
        loadComponent: () =>
          import('./features/purchase-orders/purchase-orders.component')
            .then(m => m.PurchaseOrdersComponent)
      },

      {
        path: 'inventory',
        loadComponent: () =>
          import('./features/inventory/inventory.component')
            .then(m => m.InventoryComponent)
      },

      {
        path: 'grn',
        loadComponent: () =>
          import('./features/grn/grn.component')
            .then(m => m.GrnComponent)
      },

      {
        path: 'rol',
        loadComponent: () =>
          import('./features/rol/rol.component')
            .then(m => m.RolComponent)
      },

      {
        path: 'vendors',
        loadComponent: () =>
          import('./features/vendors/vendors.component')
            .then(m => m.VendorsComponent)
      },

      { path: 'items', component: ItemsComponent },
      { path: 'warehouses', component: WarehousesComponent },

  { path: '', redirectTo: 'dashboard', pathMatch: 'full' }

    ]
  },

  { path: '**', redirectTo: 'dashboard' }
];