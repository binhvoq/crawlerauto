import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Todo } from './model/todo';
import { GetPostsQuery } from './model/query/GetPostsQuery';

import { protectedResources } from './auth-config';
import { Post } from './model/dto/Post';

@Injectable({
  providedIn: 'root',
})
export class BlogService {
  url = protectedResources.todoListApi.endpoint;

  constructor(private http: HttpClient) {}

  getTodos(getPostsQuery: GetPostsQuery) {
    return this.http.post<Post[]>(this.url + '/getpost', getPostsQuery);
  }

  getTodo(id: number) {
    return this.http.get<Todo>(this.url + '/' + id);
  }

  postTodo(todo: Todo) {
    return this.http.post<Todo>(this.url, todo);
  }

  deleteTodo(id: number) {
    return this.http.delete(this.url + '/' + id);
  }

  editTodo(todo: Todo) {
    return this.http.put<Todo>(this.url + '/' + todo.id, todo);
  }
}
