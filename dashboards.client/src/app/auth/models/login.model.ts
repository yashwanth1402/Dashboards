export interface LoginRequest {
  username: string;
  password: string;
}

export interface ResetPasswordRequest {
  username: string;
  newPassword: string;
  confirmPassword: string;
}

export interface LoginResponse {
  userId: number;
  userName: string;
  firstName: string;
  lastName: string;
  emailAddress: string | null;
  roleId: number | null;
  token: string;
  requiresPasswordReset: boolean;
}
