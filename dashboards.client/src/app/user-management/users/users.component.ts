import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../services/user.service';
import { RoleService } from '../services/role.service';
import { UserListItem, CreateUserRequest, UpdateUserRequest } from '../models/user.model';
import { RoleOption } from '../models/role.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class UsersComponent implements OnInit {
  users: UserListItem[] = [];
  filteredUsers: UserListItem[] = [];
  roleOptions: RoleOption[] = [];

  // Filters
  searchTerm = '';
  selectedRoleId: number | null = null;
  selectedStatus = 'all';

  // State
  loading = false;
  showCreateModal = false;
  isEditMode = false;
  editUserId: number | null = null;


  // Create User form
  newUser: CreateUserRequest = {
    firstName: '',
    lastName: '',
    userName:'',
    emailAddress: '',
    password:'',
    roleId: 0,
    isActive: true
  };
  createError = '';
  creating = false;
  showPassword = false;

  constructor(
    private userService: UserService,
    private roleService: RoleService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadUsers();
    this.loadRoleOptions();
  }
  goToRoles(): void {
    this.router.navigate(['user-management/roles-access']);
  }

  loadUsers(): void {
    this.loading = true;
    this.userService.getUsers(
      this.searchTerm || undefined,
      this.selectedRoleId || undefined,
      this.selectedStatus
    ).subscribe({
      next: (data) => {
        this.users = data;
        this.filteredUsers = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  loadRoleOptions(): void {
    this.userService.getRoleOptions().subscribe({
      next: (data) => this.roleOptions = data
    });
  }
  get activeUsersCount(): number {
    return this.users.filter(user => user.isActive).length;
  }

  onSearch(): void {
    this.applyFilters();
  }

  onRoleFilterChange(): void {
    this.applyFilters();
  }

  onStatusFilterChange(): void {
    this.applyFilters();
  }

  applyFilters(): void {
    let result = this.users;

    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase();
      result = result.filter(u =>
        u.firstName?.toLowerCase().includes(term) ||
        u.lastName?.toLowerCase().includes(term) ||
        u.emailAddress?.toLowerCase().includes(term) ||
        u.roleName?.toLowerCase().includes(term) ||
        u.userName?.toLowerCase().includes(term)
      );
    }

    if (this.selectedRoleId) {
      result = result.filter(u => u.roleId === this.selectedRoleId);
    }

    if (this.selectedStatus && this.selectedStatus !== 'all') {
      const isActive = this.selectedStatus === 'active';
      result = result.filter(u => u.isActive === isActive);
    }

    this.filteredUsers = result;
  }

  getInitials(user: UserListItem): string {
    return (user.firstName?.charAt(0) || '') + (user.lastName?.charAt(0) || '');
  }

  formatDate(dateStr: string | null): string {
    if (!dateStr) return '—';
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-US', { month: 'short', day: '2-digit', year: 'numeric' });
  }

  // Modal actions
  openCreateModal(): void {        
    this.isEditMode = false;
    this.editUserId = null;
    this.showCreateModal = true;
    this.createError = '';

    this.newUser = {
      firstName: '',
      lastName: '',
      userName: '',
      emailAddress: '',
      password: '',
      roleId: 0,
      isActive: true
    };
  }

 
  closeCreateModal(): void {
    this.showCreateModal = false;
    this.createError = '';
  }

  onEditUser(user: UserListItem): void {

    this.isEditMode = true;
    this.editUserId = user.userId;
    this.showCreateModal = true;
    this.createError = '';

    this.newUser = {
      firstName: user.firstName,
      lastName: user.lastName,
      userName: user.userName,
      emailAddress: user.emailAddress ?? '',
      password: '',
      roleId: user.roleId ?? 0,
      isActive: user.isActive
    };
  }

  onSaveUser(): void {

    if (!this.newUser.firstName || !this.newUser.lastName) {
      this.createError = 'First name and last name are required.';
      return;
    }

    if (!this.newUser.emailAddress) {
      this.createError = 'Email address is required.';
      return;
    }

    if (!this.isEditMode &&
      (!this.newUser.password || this.newUser.password.length < 8)) {

      this.createError = 'Password must be at least 8 characters.';
      return;
    }

    if (!this.newUser.roleId) {
      this.createError = 'Please select a user role.';
      return;
    }

    this.creating = true;
    this.createError = '';

    if (this.isEditMode) {

      const request: UpdateUserRequest = {
        firstName: this.newUser.firstName,
        lastName: this.newUser.lastName,
        userName: this.newUser.userName,
        emailAddress: this.newUser.emailAddress,
        roleId: this.newUser.roleId,
        isActive: this.newUser.isActive
      };

      this.userService.updateUser(this.editUserId!, request).subscribe({
        next: () => {
          this.creating = false;
          this.showCreateModal = false;
          this.loadUsers();
        },
        error: (err) => {
          this.creating = false;
          this.createError = err.error?.message || 'Failed to update user.';
        }
      });

    } else {

      this.userService.createUser(this.newUser).subscribe({
        next: () => {
          this.creating = false;
          this.showCreateModal = false;
          this.loadUsers();
        },
        error: (err) => {
          this.creating = false;
          this.createError = err.error?.message || 'Failed to create user.';
        }
      });

    }
  }

 
  onDeleteUser(user: UserListItem): void {
    // TODO: Confirmation dialog
    if (confirm(`Are you sure you want to delete ${user.fullName || user.firstName + ' ' + user.lastName}?`)) {
      this.userService.deleteUser(user.userId).subscribe({
        next: () => this.loadUsers()
      });
    }
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }
}
