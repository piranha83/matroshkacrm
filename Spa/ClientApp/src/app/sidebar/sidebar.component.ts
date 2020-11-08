import { Route } from '@angular/compiler/src/core';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {
  @Input() items: Object[];

  constructor(
    private router: Router) { }

  ngOnInit() {
    console.log('sidebar');
    var items = [];
    this.fromRouter(this.router, '', items);
  }


  fromRouter(router: Router, href: string = '', items: any[] = []) {
    router.config = router.config || [];
    router.config.forEach(route => {
      const label = this.getLabel(route);
      href += `/${route.path}`;
      items.push({ label, href });

      this.fromRoute(route, href, items);
      this.items.push(items);
    });
  }

  fromRoute(route: Route, href: string = '', items: any[] = []) {
    route.children = route.children || [];
    route.children.forEach(child => {
      const label = this.getLabel(child);
      const uri: string = child['path'];//.snapshot.url.map(segment => segment.path).join('/');
      if (uri !== '') href += `/${uri}`;

      items.push({ label, href });
      this.fromRoute(child, href, items);
    });
  }

  getLabel(route: any): string {
    return route.data && route.data['breadcrumb']
      ? route.data['breadcrumb']
      : route.path;
  }
}
