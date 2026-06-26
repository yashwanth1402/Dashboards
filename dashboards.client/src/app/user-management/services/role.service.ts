import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Role, RoleListItem, CreateRoleRequest, Market, ModulePermission } from '../models/role.model';

export interface PageDto {
  pageId: number;
  displayName: string;
  pageOrder: number;
}

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private readonly apiUrl = '/api/roles';

  constructor(private http: HttpClient) {}

  getRoles(search?: string): Observable<RoleListItem[]> {
    let params = new HttpParams();
    if (search) params = params.set('search', search);
    return this.http.get<RoleListItem[]>(this.apiUrl, { params });
  }

  getRole(id: number): Observable<Role> {
    return this.http.get<Role>(`${this.apiUrl}/${id}`);
  }

  createRole(request: CreateRoleRequest): Observable<any> {
    return this.http.post(this.apiUrl, request);
  }

  updateRole(id: number, request: CreateRoleRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, request);
  }

  deleteRole(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getRoleOptions(): Observable<{ roleId: number; name: string }[]> {
    return this.http.get<{ roleId: number; name: string }[]>(`${this.apiUrl}/options`);
  }

  getMarkets(): Observable<Market[]> {
    return this.http.get<Market[]>('/api/markets');
  }

  getPages(): Observable<PageDto[]> {
    return this.http.get<PageDto[]>('/api/pages');
  }

  getModulePermissions(userTypeId: number): Observable<ModulePermission[]> {
    return this.http.get<ModulePermission[]>(`/api/pages/permissions/${userTypeId}`);
  }

  getModulePermissionsByRole(roleId: number): Observable<ModulePermission[]> {
    return this.http.get<ModulePermission[]>(`/api/pages/permissions/by-role/${roleId}`);
  }
}
