import { useEffect, useState } from "react";
import ListGroup from "react-bootstrap/ListGroup";
import { Link } from "react-router-dom";
import { getRandomPosts } from "../../Services/Widgets";

const RandomPostsWidget = () => {
  const [randomPostsList, setRandomPostsList] = useState([]);

  useEffect(() => {
    getRandomPosts(5).then((data) => {
      if (data) {
        setRandomPostsList(data);
      } else setRandomPostsList([]);
    });
  }, []);
  return (
    <div className="mb-4">
      <h3 className="text-success mb-2 text-success">Bài viết ngẫu nhiên</h3>
      {randomPostsList.length > 0 && (
        <ListGroup>
          {randomPostsList.map((item, index) => {
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

export default RandomPostsWidget;
