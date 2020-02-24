import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {RouterModule} from '@angular/router';
import { ProductFormComponent } from './components/product-form/product-form.component';



@NgModule({
  declarations: [ProductFormComponent],
  imports: [
    CommonModule,
    RouterModule.forChild([{
      path: '',
      component: ProductFormComponent
    }])
  ]
})
export class ProductEditorModule { }
