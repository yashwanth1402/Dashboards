import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RoleService, PageDto } from '../services/role.service';
import { RoleListItem, CreateRoleRequest, Market, ModulePermission } from '../models/role.model';

@Component({
  selector: 'app-roles-access',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './roles-access.component.html',
  styleUrls: ['./roles-access.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class RolesAccessComponent implements OnInit {
  roles: RoleListItem[] = [];
  filteredRoles: RoleListItem[] = [];
  searchTerm = '';
  loading = false;

  // Modal state
  showModal = false;
  isEditMode = false;
  editRoleId: number | null = null;
  modalError = '';
  saving = false;

  // Form fields
  roleName = '';
  roleDescription = '';

  // Markets
  markets: Market[] = [];
  selectedMarketIds: number[] = [];
  showMarketDropdown = false;

  // Module Permissions
  pages: PageDto[] = [];
  modulePermissions: ModulePermission[] = [];

  constructor(private roleService: RoleService, private router: Router) {}

  ngOnInit(): void {
    this.loadRoles();
    this.loadMarkets();
    this.loadPages();
  }

  goToUsers(): void {
    this.router.navigate(['user-management/users']);
  }

  // --- DATA LOADING ---
  loadRoles(): void {
    this.loading = true;
    this.roleService.getRoles().subscribe({
      next: (data) => {
        this.roles = data;
        this.filteredRoles = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  loadMarkets(): void {
    this.roleService.getMarkets().subscribe({
      next: (data) => { this.markets = data; }
    });
  }

  loadPages(): void {
    this.roleService.getPages().subscribe({
      next: (data) => {
        this.pages = data;
      }
    });
  }

  onSearch(): void {
    if (!this.searchTerm.trim()) {
      this.filteredRoles = this.roles;
    } else {
      const term = this.searchTerm.toLowerCase();
      this.filteredRoles = this.roles.filter(r =>
        r.name.toLowerCase().includes(term)
      );
    }
  }

  // --- CREATE ---
  onNewRole(): void {
    this.isEditMode = false;
    this.editRoleId = null;
    this.roleName = '';
    this.roleDescription = '';
    this.selectedMarketIds = [];
    this.showMarketDropdown = false;
    this.modalError = '';
    this.initPermissions();
    this.showModal = true;
  }

  // --- EDIT ---
  onEditRole(role: RoleListItem): void {
    this.isEditMode = true;
    this.editRoleId = role.roleId;
    this.roleName = role.name;
    this.roleDescription = role.description || '';
    this.selectedMarketIds = [];
    this.showMarketDropdown = false;
    this.modalError = '';
    this.initPermissions();

    // Load existing permissions for this role
    this.roleService.getModulePermissionsByRole(role.roleId).subscribe({
      next: (perms) => {
        // Merge existing permissions into our grid
        perms.forEach(p => {
          const mod = this.modulePermissions.find(m => m.pageId === p.pageId);
          if (mod) {
            mod.canAdd = p.canAdd;
            mod.canEdit = p.canEdit;
            mod.canDelete = p.canDelete;
            mod.canView = p.canView;
          }
        });
      }
    });

    // Match market names to IDs
    if (role.markets && role.markets.length > 0) {
      this.selectedMarketIds = this.markets
        .filter(m => role.markets.includes(m.name))
        .map(m => m.marketId);
    }

    this.showModal = true;
  }

  // --- DELETE ---
  onDeleteRole(role: RoleListItem): void {
    if (confirm(`Are you sure you want to delete the role "${role.name}"?`)) {
      this.roleService.deleteRole(role.roleId).subscribe({
        next: () => { this.loadRoles(); },
        error: () => { alert('Failed to delete role.'); }
      });
    }
  }

  // --- MODAL ACTIONS ---
  closeModal(): void {
    this.showModal = false;
    this.modalError = '';
  }

  onSaveRole(): void {
    if (!this.roleName.trim()) {
      this.modalError = 'Role name is required.';
      return;
    }

    this.saving = true;
    this.modalError = '';

    const request: CreateRoleRequest = {
      name: this.roleName.trim(),
      description: this.roleDescription.trim() || null,
      marketIds: this.selectedMarketIds,
      modulePermissions: this.modulePermissions
    };

    if (this.isEditMode && this.editRoleId) {
      this.roleService.updateRole(this.editRoleId, request).subscribe({
        next: () => {
          this.saving = false;
          this.showModal = false;
          this.loadRoles();
        },
        error: () => {
          this.saving = false;
          this.modalError = 'Failed to update role.';
        }
      });
    } else {
      this.roleService.createRole(request).subscribe({
        next: () => {
          this.saving = false;
          this.showModal = false;
          this.loadRoles();
        },
        error: () => {
          this.saving = false;
          this.modalError = 'Failed to create role.';
        }
      });
    }
  }

  // --- MARKETS MULTI-SELECT ---
  toggleMarketDropdown(): void {
    this.showMarketDropdown = !this.showMarketDropdown;
  }

  isMarketSelected(marketId: number): boolean {
    return this.selectedMarketIds.includes(marketId);
  }

  toggleMarket(marketId: number): void {
    const idx = this.selectedMarketIds.indexOf(marketId);
    if (idx > -1) {
      this.selectedMarketIds.splice(idx, 1);
    } else {
      this.selectedMarketIds.push(marketId);
    }
  }

  getSelectedMarketNames(): string {
    if (this.selectedMarketIds.length === 0) return '';
    return this.markets
      .filter(m => this.selectedMarketIds.includes(m.marketId))
      .map(m => m.name)
      .join(', ');
  }

  // --- MODULE PERMISSIONS ---
  initPermissions(): void {
    this.modulePermissions = this.pages.map(p => ({
      pageId: p.pageId,
      moduleName: p.displayName,
      canAdd: false,
      canEdit: false,
      canDelete: false,
      canView: false
    }));
  }

  selectAllPermissions(): void {
    this.modulePermissions.forEach(m => {
      m.canAdd = true;
      m.canEdit = true;
      m.canDelete = true;
      m.canView = true;
    });
  }

  clearAllPermissions(): void {
    this.modulePermissions.forEach(m => {
      m.canAdd = false;
      m.canEdit = false;
      m.canDelete = false;
      m.canView = false;
    });
  }
}
