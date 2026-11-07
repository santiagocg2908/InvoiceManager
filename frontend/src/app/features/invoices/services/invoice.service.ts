import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../../core/config/api.config';

import type { InvoiceListItem } from '../models/invoice-list-item.model';
import type { PagedResult } from '../../../shared/models/paged-result.model';
import type { InvoiceListQuery } from '../models/invoice-list-query.model';
import type { InvoiceDetail } from '../models/invoice-detail.model';
import type { UpdateInvoiceRequest } from '../models/update-invoice-request.model';
import type { InvoiceSummary } from '../models/invoice-summary.model';
import type { InvoiceUploadRequest } from '../models/invoice-upload-request.model';

@Injectable({
    providedIn: 'root'
})

export class InvoiceService{

    constructor(
        private readonly http: HttpClient
    )
    {
    
    }

    getInvoices(query: InvoiceListQuery={}
    ): Observable<PagedResult<InvoiceListItem>>{
        const url = `${API_BASE_URL}/facturas`;

        let params = new HttpParams();

        if (query.page !== undefined) {
            params = params.set('page', query.page);
        }

        if (query.pageSize !== undefined) {
            params = params.set('pageSize', query.pageSize);
        }

        if (query.supplier?.trim()) {
            params = params.set('supplier', query.supplier.trim());
        }

        if (query.issueDateFrom) {
            params = params.set('issueDateFrom', query.issueDateFrom);
        }

        if (query.issueDateTo) {
            params = params.set('issueDateTo', query.issueDateTo);
        }

        if (query.includeInactive !== undefined) {
            params = params.set('includeInactive', query.includeInactive);
        }

        return this.http.get<PagedResult<InvoiceListItem>>(
            url,
            { params }
        );
    }

    getInvoiceById(id:number): Observable<InvoiceDetail>{
        const url = `${API_BASE_URL}/facturas/${id}`;

        return this.http.get<InvoiceDetail>(url);
    }

    updateInvoice(id:number, request: UpdateInvoiceRequest): Observable<InvoiceDetail>{
        const url = `${API_BASE_URL}/facturas/${id}`;
        return this.http.put<InvoiceDetail>(url, request);
    }

    desactiveInvoice(id:number): Observable<void> {
        const url = `${API_BASE_URL}/facturas/${id}`;

        return this.http.delete<void>(url);
    }
    
    getInvoiceSummary(): Observable<InvoiceSummary[]>{
        const url = '${API_BASE_URL}/facturas/reporte/resumen';

        return this.http.get<InvoiceSummary[]>(url);
    }

    uploadInvoice(request: InvoiceUploadRequest): Observable<unknown>{
        const formData = new FormData();
        
        formData.append('file', request.file, request.file.name);
        const url = `${API_BASE_URL}/facturas`;

        return this.http.post<unknown>(url, formData);
    }
}