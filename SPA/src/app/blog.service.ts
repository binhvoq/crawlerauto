import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Todo } from './model/todo';
import { GetPostsQuery } from './model/query/GetPostsQuery';

import { protectedResources } from './auth-config';
import { ChangeListDto, GetPostsDto, Post } from './model/dto/Post';

@Injectable({
  providedIn: 'root',
})
export class BlogService {
  url = protectedResources.todoListApi.endpoint;

  constructor(private http: HttpClient) {}

  getPosts(getPostsQuery: GetPostsQuery) {
    return this.http.post<GetPostsDto>(this.url + '/getpost', getPostsQuery);
  }

  addPosts() {
    return this.http.post<Post[]>(this.url + '/addpost', {});
  }

  addComments() {
    return this.http.post<ChangeListDto[]>(this.url + '/addcmts', {});
  }
}
