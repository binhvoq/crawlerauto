import { Component, OnInit } from '@angular/core';
import { ChangeListDto, Post } from '../model/dto/Post';
import { BlogService } from '../blog.service';

@Component({
  selector: 'app-todo-view',
  templateUrl: './todo-view.component.html',
  styleUrls: ['./todo-view.component.css'],
})
export class TodoViewComponent implements OnInit {
  loadingGetPosts: boolean = false;
  loadingUpdateComments: boolean = false;

  returnDataGetPosts: Post[] = [];
  returnDataGetComments: ChangeListDto[] = [];

  constructor(private service: BlogService) {}

  ngOnInit(): void {}
  getPosts(): void {
    this.loadingGetPosts = true;
    this.service.addPosts().subscribe((data) => {
      this.returnDataGetPosts = data;
      this.loadingGetPosts = false;
    });
  }

  getComments(): void {
    this.loadingUpdateComments = true;

    this.service.addComments().subscribe((data) => {
      this.returnDataGetComments = data;
      this.loadingUpdateComments = false;
    });
  }
}
