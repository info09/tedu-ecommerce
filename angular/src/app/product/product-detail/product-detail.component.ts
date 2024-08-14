import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import {
  ProductCategoriesService,
  ProductCategoryInListDto,
} from '@proxy/tedu-ecommerce/product-categories';
import { ProductDto, ProductsService } from '@proxy/tedu-ecommerce/products';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
})
export class ProductDetailComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  blockedPanel: boolean = false;
  public form: FormGroup;
  //Dropdown
  productCategories: any[] = [];
  selectedEntity = {} as ProductDto;
  constructor(
    private productsService: ProductsService,
    private productCategoriesService: ProductCategoriesService,
    private fb: FormBuilder
  ) {}
  ngOnDestroy(): void {
    throw new Error('Method not implemented.');
  }
  ngOnInit(): void {
    this.buildForm();
  }

  loadFormDetail(id: string) {
    this.toggleBlockUI(true);
    this.productsService
      .get(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (res: ProductDto) => {
          this.selectedEntity = res;
          this.buildForm();
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  saveChange() {}

  loadProductCategories() {
    this.productCategoriesService
      .getListAll()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((res: ProductCategoryInListDto[]) => {
        res.forEach(element => {
          this.productCategories.push({
            value: element.id,
            name: element.name,
          });
        });
      });
  }

  private buildForm() {
    this.form = this.fb.group({
      name: new FormControl(this.selectedEntity.name || null, Validators.required),
    });
  }

  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.blockedPanel = true;
    } else {
      setTimeout(() => {
        this.blockedPanel = false;
      }, 1000);
    }
  }
}
