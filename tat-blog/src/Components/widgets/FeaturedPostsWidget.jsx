import { useEffect, useState } from "react";
import ListGroup from "react-bootstrap/ListGroup";
import { Link } from "react-router-dom";
import { getFeaturedPosts } from "../../Services/Widgets";

const FeaturedPostsWidget = () => {
  const [featuredPostsList, setFeaturedPostsList] = useState([]);

  useEffect(() => {
    getFeaturedPosts(3).then((data) => {
      if (data) {
        setFeaturedPostsList(data);
      } else setFeaturedPostsList([]);
    });
  }, []);
  return (
    <div className="mb-4">
      <h3 className="text-success mb-2 text-success">Bài viết nổi bật</h3>
      {featuredPostsList.length > 0 && (
        <ListGroup>
          {featuredPostsList.map((item, index) => {
            return (
              <ListGroup.Item key={index}>
                <Link to={`/blog/post/?slug=${item.urlSlug}`} key={index}>
                  {item.title}
                </Link>
              </ListGroup.Item>
            );
          })}
        </ListGroup>
      )}
    </div>
  );
};

export default FeaturedPostsWidget;
