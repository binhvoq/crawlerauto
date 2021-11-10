import { Component, OnInit } from '@angular/core';

import { BlogService } from '../blog.service';

@Component({
  selector: 'app-todo-view',
  templateUrl: './todo-view.component.html',
  styleUrls: ['./todo-view.component.css'],
})
export class TodoViewComponent implements OnInit {
  loadingGetPosts: boolean = false;
  loadingUpdateComments: boolean = false;

  constructor(private service: BlogService) {}

  ngOnInit(): void {}
  getPosts(): void {
    this.loadingGetPosts = true;
  }

  getComments(): void {
    this.loadingUpdateComments = true;
  }
}
