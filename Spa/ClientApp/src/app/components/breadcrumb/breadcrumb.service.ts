import { Injectable } from '@angular/core';
import { ActivatedRoute, Route, Router, Routes } from '@angular/router';
import { BreadCrumbItem } from './breadcrumb.model';

@Injectable()
export class BreadCrumbService {

  public create(root: ActivatedRoute): BreadCrumbItem[] {
    var href: string = '';
    var items: BreadCrumbItem[] = [];
    return this.getItems(root, href, items);
  }

  private getItems(route: ActivatedRoute, href: string = '#', breadcrumbs: BreadCrumbItem[] = []): BreadCrumbItem[] {
    const children: ActivatedRoute[] = route.children;

    if (children.length === 0) {
      return breadcrumbs;
    }

    for (const child of children) {
      const routeURL: string = child.snapshot.url.map(segment => segment.path).join('/');
      const label = child.snapshot.data["breadcrumb"] || routeURL;
      href += `/${routeURL}`;
      
      if(label!='')
        breadcrumbs.push({label, href});
      return this.getItems(child, href, breadcrumbs);
    }
  }
}