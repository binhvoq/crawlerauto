import { Component, OnInit } from '@angular/core';
import { GetPostsDto, Post } from '../model/dto/Post';
import { BlogService } from '../blog.service';
import appSetting from '../appsetting';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  currentPage: number = 1;
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

  nextPage(): void {
    if (this.currentPage < this.postsData.totalPages) {
      this.currentPage++;
      this.getPosts();
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.getPosts();
    }
  }

  changePage(event: any): void {
    const pageNumber = parseInt(event.target.value);
    if (pageNumber > 0 && pageNumber <= this.postsData.totalPages)
      this.currentPage = pageNumber;
    this.getPosts();
  }

  getPosts(): void {
    this.service
      .getPosts({
        pageIndex: this.currentPage,
        pageSize: appSetting.pageSize,
      })
      .subscribe((data) => {
        this.postsData = data;
      });
  }
}
