import { Component, OnInit } from '@angular/core';
import { GetPostsDto, Post } from '../model/dto/Post';
import { BlogService } from '../blog.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  postsData: GetPostsDto = {
    posts: [] as Post[],
    totalPages: 0,
  };

  constructor(private service: BlogService) {}

  ngOnInit(): void {
    setTimeout(() => {
      this.getPosts();
    }, 2000);
  }

  getPosts(): void {
    this.service.getPosts({ pageIndex: 1, pageSize: 10 }).subscribe((data) => {
      this.postsData = data;
    });
  }

  // ngOnDestroy(): void {

  // }
}
