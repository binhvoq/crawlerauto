import { Component, OnInit } from '@angular/core';
import { Post } from '../model/dto/Post';
import { BlogService } from '../blog.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  post: Post[] = [];

  constructor(private service: BlogService) {}

  ngOnInit(): void {
    this.getPost();
  }

  getPost(): void {
    this.service.getTodos({ pageIndex: 1, pageSize: 1 }).subscribe((data) => {
      console.log(data);
      this.post = data;
    });
  }

  // ngOnDestroy(): void {

  // }
}
