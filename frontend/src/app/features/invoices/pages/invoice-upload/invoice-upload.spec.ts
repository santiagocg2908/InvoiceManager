import { ComponentFixture, TestBed } from '@angular/core/testing';
import { InvoiceUpload } from './invoice-upload';

describe('InvoiceUpload', () => {
  let component: InvoiceUpload;
  let fixture: ComponentFixture<InvoiceUpload>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InvoiceUpload],
    }).compileComponents();

    fixture = TestBed.createComponent(InvoiceUpload);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
