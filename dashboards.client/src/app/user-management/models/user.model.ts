export interface UserListItem {
  userId: number;
  userName: string;
  firstName: string;
  lastName: string;
  fullName: string;
  emailAddress: string | null;
  roleName: string | null;
  roleId: number | null;
  createdBy: string | null;
  createdDate: string | null;
  isActive: boolean;
}

export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  userName: string;
  emailAddress: string;
  password: string;
  roleId: number;
  isActive: boolean;
}

export interface UpdateUserRequest {
  firstName: string;
  lastName: string;
  userName: string;
  emailAddress: string | null;
  roleId: number | null;
  isActive: boolean;
}
