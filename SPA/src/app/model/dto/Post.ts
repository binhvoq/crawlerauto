import {Comment} from "./Comment"

export interface Post {
    id: string;
    uri: string;
    title: string;
    summary: string;
    totalComments: string;
    comments: Array<Comment>;
}