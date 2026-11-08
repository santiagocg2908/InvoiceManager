import { Routes } from '@angular/router';

import {InvoiceList} from './features/invoices/pages/invoice-list/invoice-list';
import { InvoiceDetailComponent } from
  './features/invoices/pages/invoice-detail/invoice-detail';import {InvoiceUpload} from './features/invoices/pages/invoice-upload/invoice-upload';


export const routes: Routes = [
    //invoice list
    {
        path: 'facturas',
        component: InvoiceList,
        title: 'Facturas'
    },

    //Upload documents
    {
        path: 'facturas/cargar',
        component: InvoiceUpload,
        title: 'Cargar factura'
    },

    {
        path: 'facturas/:id',
        component: InvoiceDetailComponent,
        title: 'Detalle de factura'
    },

    {
        path: '',
        redirectTo: 'facturas',
        pathMatch: 'full'
    },

    {
        path: '**',
        redirectTo: 'facturas'
    }
];
