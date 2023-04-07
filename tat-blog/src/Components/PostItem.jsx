import TagsList from "./TagList";
import Card from "react-bootstrap/Card";
import { Link } from "react-router-dom";
import { isEmptyOrSpaces } from "../Utils/Utils";

const PostsLists = ({ postItem }) => {
  let imageUrl = isEmptyOrSpaces(postItem.imageUrl)
    ? process.env.PUBLIC_URL + "/images/image_1.jpg"
    : `${postItem.imageUrl}`;

  let postedDateString = new Date(postItem.postedDate).toDateString();

  let postedDate = new Date(postedDateString.slice(3, 15));

  return (
    <article className="blog-entry mb-4">
      <Card>
        <div className="row g-0">
          <div className="col-md-4">
            <Card.Img variant="top" src={imageUrl} alt={postItem.title} />
          </div>
          <div className="col-md-8">
            <Card.Body>
              <Card.Title>{postItem.title}</Card.Title>
              <Card.Text>
                <small className="text-muted">Tác giả:</small>
                <Link
                  to={`/blog/author/${postItem.author.urlSlug}`}
                  className="text-primary text-decoration-none m-1"
                >
                  {postItem.author.fullName}
                </Link>
                <small className="text-muted">Chủ đề:</small>
                <Link
                  to={`/blog/category/${postItem.category.urlSlug}`}
                  className="text-primary text-decoration-none m-1"
                >
                  {postItem.category.name}
                </Link>
              </Card.Text>
              <Card.Text>{postItem.shortDescription}</Card.Text>
              <div className="tag-list">
                <TagsList tagsList={postItem.tags} />
              </div>
              <div className="text-end">
                <Link
                  to={`/blog/post/?year=${postedDate.getFullYear()}&month=${postedDate.getMonth()}&day=${postedDate.getDay()}&slug=${
                    postItem.urlSlug
                  }`}
                  className="btn btn-primary"
                  title={postItem.title}
                >
                  Xem chi tiết
                </Link>
              </div>
            </Card.Body>
          </div>
        </div>
      </Card>
    </article>
  );
};

export default PostsLists;
