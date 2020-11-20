import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { TreeNode } from './treenode.type';

@Injectable({ providedIn: 'root' })
export class TreeNodeService {
    selectedTreeNode: TreeNode;

    set selected(selectedTreeNode: TreeNode) {
        this.selectedTreeNode = selectedTreeNode;
    }

    get root(): TreeNode {
        var node = {
            name: '', children:  [
                { name: 'a', children: null, root: '' },
                { name: 'b', children: null, root: '' },
                {
                    name: 'c', children: [
                        { name: 'c1', children: null, root: 'c' },
                        { name: 'c2', children: null, root: 'c' },
                    ]
                },
            ]
        };
        return node;
    }

    load(name: string): Observable<TreeNode[]> {
        return new BehaviorSubject<TreeNode[]>(this.root.children).asObservable();
    }

    find(name: string): Observable<TreeNode[]> {
        return new BehaviorSubject<TreeNode[]>(this.root.children).asObservable();
    }
}