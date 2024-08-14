import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { ProductDto, ProductInListDto, ProductsService } from '@proxy/tedu-ecommerce/products';
import { PagedResultDto } from '@abp/ng.core';
import {
  ProductCategoriesService,
  ProductCategoryInListDto,
} from '@proxy/tedu-ecommerce/product-categories';
import { NotificationService } from '../shared/services/notificationService.service';
import { DialogService } from 'primeng/dynamicdialog';
import { ProductDetailComponent } from './product-detail/product-detail.component';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.scss'],
})
export class ProductComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();
  blockedPanel: boolean = false;
  items: ProductInListDto[] = [];
  //Paging variables
  public skipCount: number = 0;
  public maxResultCount: number = 10;
  public totalCount: number;
  //Filter
  productCategories: any[] = [];
  keyword: string = '';
  categoryId: string = '';

  constructor(
    private productsService: ProductsService,
    private productCategoriesService: ProductCategoriesService,
    private dialogService: DialogService,
    private notificationService: NotificationService
  ) {}
  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
  ngOnInit(): void {
    this.loadData();
    this.loadProductCategories();
  }

  loadData() {
    this.toggleBlockUI(true);
    this.productsService
      .getListFilter({
        keyword: '',
        categoryId: this.categoryId,
        maxResultCount: this.maxResultCount,
        skipCount: this.skipCount,
      })
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (res: PagedResultDto<ProductInListDto>) => {
          this.items = res.items;
          this.totalCount = res.totalCount;
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  loadProductCategories() {
    this.productCategoriesService
      .getListAll()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (res: ProductCategoryInListDto[]) => {
          res.forEach(element => {
            this.productCategories.push({
              value: element.id,
              name: element.name,
            });
          });
        },
      });
  }

  pageChanged(event: any) {
    this.skipCount = (event.page - 1) * this.maxResultCount;
    this.maxResultCount = event.rows;
    this.loadData();
  }

  showAddModal() {
    const ref = this.dialogService.open(ProductDetailComponent, {
      header: 'Thêm mới sản phẩm',
      width: '70%',
    });

    ref.onClose.subscribe((data: ProductDto) => {
      if (data) {
        this.loadData();
        this.notificationService.showSuccess('Thêm sản phẩm thành công');
      }
    });
  }

  private toggleBlockUI(enable: boolean) {
    if (enable == true) {
      this.blockedPanel = true;
    } else {
      setTimeout(() => {
        this.blockedPanel = false;
      }, 1000);
    }
  }
}
