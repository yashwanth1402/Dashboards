export interface Role {
  roleId: number;
  name: string;
  isDeleted: boolean;
  createdBy: string | null;
  createdDate: string | null;
  modifiedBy: string | null;
  modifiedDate: string | null;
}

export interface RoleListItem {
  roleId: number;
  name: string;
  description: string | null;
  markets: string[];
  userCount: number;
  createdDate: string | null;
}

export interface CreateRoleRequest {
  name: string;
  description: string | null;
  marketIds: number[];
  modulePermissions: ModulePermission[];
}

export interface ModulePermission {
  pageId: number;
  moduleName: string;
  canAdd: boolean;
  canEdit: boolean;
  canDelete: boolean;
  canView: boolean;
}

export interface RoleOption {
  roleId: number;
  name: string;
}

export interface Market {
  marketId: number;
  name: string;
  code: string;
}
