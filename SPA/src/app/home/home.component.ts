import { Component, OnInit } from '@angular/core';
import { Post } from '../model/dto/Post';
import { BlogService } from '../blog.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  posts: Post[] = [];

  constructor(private service: BlogService) {}

  ngOnInit(): void {
    setTimeout(() => {
      this.getPost();
    }, 1000);
  }

  getPost(): void {
    this.service.getTodos({ pageIndex: 1, pageSize: 10 }).subscribe((data) => {
      this.posts = data;
    });
  }

  // ngOnDestroy(): void {

  // }
}
