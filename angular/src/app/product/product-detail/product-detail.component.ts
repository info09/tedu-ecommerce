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
  manufacturers: any[] = [];
  productTypes: any[] = [];
  selectedEntity = {} as ProductDto;
  btnDisabled = false;
  constructor(
    private productsService: ProductsService,
    private productCategoriesService: ProductCategoriesService,
    private fb: FormBuilder
  ) {}

  validationMessages = {
    code: [{ type: 'required', message: 'Bạn phải nhập mã duy nhất' }],
    name: [
      { type: 'required', message: 'Bạn phải nhập tên' },
      { type: 'maxlength', message: 'Bạn không được nhập quá 255 kí tự' },
    ],
    slug: [{ type: 'required', message: 'Bạn phải URL duy nhất' }],
    sku: [{ type: 'required', message: 'Bạn phải mã SKU sản phẩm' }],
    manufacturerId: [{ type: 'required', message: 'Bạn phải chọn nhà cung cấp' }],
    categoryId: [{ type: 'required', message: 'Bạn phải chọn danh mục' }],
    productType: [{ type: 'required', message: 'Bạn phải chọn loại sản phẩm' }],
    sortOrder: [{ type: 'required', message: 'Bạn phải nhập thứ tự' }],
    sellPrice: [{ type: 'required', message: 'Bạn phải nhập giá bán' }],
  };

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
      code: new FormControl(this.selectedEntity.code || null, Validators.required),
      slug: new FormControl(this.selectedEntity.slug || null, Validators.required),
      sku: new FormControl(this.selectedEntity.sku || null, Validators.required),
      manufacturerId: new FormControl(
        this.selectedEntity.manufacturerId || null,
        Validators.required
      ),
      categoryId: new FormControl(this.selectedEntity.categoryId || null, Validators.required),
      productType: new FormControl(this.selectedEntity.productType || null, Validators.required),
      sortOrder: new FormControl(this.selectedEntity.sortOrder || null, Validators.required),
      sellPrice: new FormControl(this.selectedEntity.sellPrice || null, Validators.required),
      visibility: new FormControl(this.selectedEntity.visibility || false),
      isActive: new FormControl(this.selectedEntity.isActive || false),
      seoMetaDescription: new FormControl(this.selectedEntity.seoMetaDescription || null),
      description: new FormControl(this.selectedEntity.description || null),
    });
  }

  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.blockedPanel = true;
      this.btnDisabled = true;
    } else {
      setTimeout(() => {
        this.blockedPanel = false;
        this.btnDisabled = true;
      }, 1000);
    }
  }
}
