import { Route } from '@angular/compiler/src/core';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {
  @Input() items: Object[] = [];

  constructor(
    private router: Router) { }

  ngOnInit() {
    console.log('sidebar');    
    this.fromRouter(this.router);
  }


  fromRouter(router: Router) {
    var data = localStorage.getItem("sidebar");
    var items : any[] = data ? JSON.parse(data) : [];
    if(items.length == 0) {
      router.config = router.config || [];
      router.config.forEach(route => {
        const label = this.getLabel(route);
        var href = `/${route.path}`;

        if (href !== '' && href != '**' && label != '**') {
          items.push({ label, href });
          this.fromRoute(route, href, items);
        }
      });
      localStorage.setItem('sidebar', JSON.stringify(items));
    }
    this.items = items;
  }

  fromRoute(route: Route, root: string = '', items: any[]) {
    route.children = route.children || [];
    route.children.forEach(child => {
      var href = root;
      const label = this.getLabel(child);
      const uri: string = child['path'];
      href += `/${uri}`;

      if (uri !== '' && uri != '**' && label != '**') {
        items.push({ label, href });
        this.fromRoute(child, href, items);
      }
    });
  }

  getLabel(route: any): string {
    return route.data && route.data['breadcrumb']
      ? route.data['breadcrumb']
      : route.path;
  }
}
