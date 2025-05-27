export interface Pagination {
  pageIndex: number
  pageSize: number
  totalRow: number
  pageCount: number
}

export enum FilterLogicalOperator {
  And = 0,
  Or = 1
}

export enum FilterOperator {
  Equal = 0,
  NotEqual = 1,
  LessThan = 2,
  LessThanOrEqual = 3,
  GreaterThan = 4,
  GreaterThanOrEqual = 5,
  Contains = 6,
  NotContains = 7,
  StartsWith = 8,
  EndsWith = 9,
  IsNull = 10,
  IsNotNull = 11,
  In = 12,
  NotIn = 13
}

export enum SortDirection {
  Ascending = 0,
  Descending = 1
}

export interface FilterDetailsRequest {
  field?: string
  operator: FilterOperator
  value?: any
}

export interface FilterRequest {
  logicalOperator: FilterLogicalOperator
  details?: FilterDetailsRequest[]
}

export interface SortDescriptor {
  field?: string
  direction: SortDirection
}

export interface FilterBodyRequest {
  langId: string
  searchValue: string
  filter: FilterRequest | {}
  orders: SortDescriptor[]
  pagination: Pagination
}

export interface ApiResponse<T> {
  data: T[]
  pagination: Pagination
}

export interface ApiError {
  message: string
  statusCode: number
  errors?: Record<string, string[]>
} 