import { DateOnly } from './date-only';

export interface InvoiceListQuery {
  page?: number;
  pageSize?: number;
  supplier?: string;
  issueDateFrom?: DateOnly;
  issueDateTo?: DateOnly;
  includeInactive?: boolean;
}