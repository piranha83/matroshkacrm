import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { TreeNode } from './treenode.type';

@Component({
  selector: 'treenode',
  templateUrl: './treenode.component.html',
  styleUrls: ['./treenode.component.css']
})
export class TreeNodeComponent implements OnInit, OnDestroy {
  @Input() Item: TreeNode;
  @Output() onTreeNodeSelect: EventEmitter<TreeNode> = new EventEmitter<TreeNode>();
  @Output() onTreeNodeLoadChildren: EventEmitter<TreeNode> = new EventEmitter<TreeNode>();
  @Output() onTreeNodeSave: EventEmitter<TreeNode> = new EventEmitter<TreeNode>();
  @Output() onTreeNodeRemove: EventEmitter<TreeNode> = new EventEmitter<TreeNode>();

  constructor() { }

  ngOnInit() {
    console.log('tree');
    if(this.Item && this.Item.children) {
      this.Item.children.map((child)=> child.parent = this.Item);
    }
  }

  ngOnDestroy() {
    this.onTreeNodeSelect.unsubscribe();
    this.onTreeNodeLoadChildren.unsubscribe();
    this.onTreeNodeSave.unsubscribe();
    this.onTreeNodeRemove.unsubscribe();
  }

  select(treeNode: TreeNode) {
    this.onTreeNodeSelect.emit(treeNode);
  }

  expand(treeNode: TreeNode) {
    treeNode.expanded = !treeNode.expanded;
    if (treeNode.expanded && !treeNode.children) {
      this.onTreeNodeLoadChildren.emit(treeNode);
    }
  }

  children(treeNode: TreeNode) {
    this.onTreeNodeLoadChildren.emit(treeNode);
  }

  save(treeNode: TreeNode) {
    this.onTreeNodeSave.emit(treeNode);
  }

  remove(treeNode: TreeNode) { 
    if(treeNode) {   
      this.onTreeNodeRemove.emit(treeNode);
    }
  }


  allowDrop(ev) {
    ev.preventDefault();
  }

  drag(ev) {
    ev.dataTransfer.setData("text", ev.target.id);
  }

  drop(ev) {
    ev.preventDefault();
    var data = ev.dataTransfer.getData("text");
    if(ev.target.tagName != "LI") {
      return;
    }
    ev.target.parentNode.insertBefore(document.getElementById(data), ev.target.nextSibling);
    //ev.target.insertAfter(document.getElementById(data));
  }
}
