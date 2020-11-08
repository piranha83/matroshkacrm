import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs/operators';
import { BreadCrumbItem } from './breadcrumb.model';
import { BreadCrumbService } from './breadcrumb.service';

@Component({
  providers: [BreadCrumbService],
  selector: 'breadcrumbs',
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.css']
})
export class BreadcrumbComponent implements OnInit {
  items: BreadCrumbItem[];

  constructor(
    private router: Router,
    private breadCrumbService: BreadCrumbService,
    private activatedRoute: ActivatedRoute) {}  

  ngOnInit(): void {    
    console.log('breadcrumb');
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.items = this.breadCrumbService.create(this.activatedRoute.root);
      });
  }
}
