import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserListItem, CreateUserRequest, UpdateUserRequest } from '../models/user.model';
import { RoleOption } from '../models/role.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly apiUrl = '/api/users';

  constructor(private http: HttpClient) {}

  getUsers(search?: string, roleId?: number, status?: string): Observable<UserListItem[]> {
    let params = new HttpParams();
    if (search) params = params.set('search', search);
    if (roleId) params = params.set('roleId', roleId.toString());
    if (status && status !== 'all') params = params.set('status', status);
    return this.http.get<UserListItem[]>(this.apiUrl, { params });
  }

  getUser(id: number): Observable<UserListItem> {
    return this.http.get<UserListItem>(`${this.apiUrl}/${id}`);
  }

  createUser(request: CreateUserRequest): Observable<UserListItem> {
    return this.http.post<UserListItem>(this.apiUrl, request);
  }

  updateUser(id: number, request: UpdateUserRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, request);
  }

  deleteUser(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getRoleOptions(): Observable<RoleOption[]> {
    return this.http.get<RoleOption[]>('/api/roles/options');
  }
}
