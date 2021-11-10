import { Comment } from './Comment';

export interface Post {
  id: string;
  uri: string;
  title: string;
  summary: string;
  totalComments: string;
  comments: Array<Comment>;
}

export interface GetPostsDto {
  posts: Post[];
  totalPages: number;
}

export interface ChangeListDto {
  id: string;
  title: string;
  uri: string;
  commentsUpdated: string;
}
