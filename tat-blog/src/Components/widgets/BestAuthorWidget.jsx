import { useEffect, useState } from "react";
import ListGroup from "react-bootstrap/ListGroup";
import { Link } from "react-router-dom";
import { getPopularAuthors } from "../../Services/Widgets";

const PopularAuthors = () => {
  const [authorsList, setAuthorsList] = useState([]);

  useEffect(() => {
    getPopularAuthors(4).then((data) => {
      if (data) {
        setAuthorsList(data);
      } else setAuthorsList([]);
    });
  }, []);
  return (
    <div className="mb-4">
      <h3 className="text-success mb-2 text-success">Tác giả nổi tiếng</h3>
      {authorsList.length > 0 && (
        <ListGroup>
          {authorsList.map((item, index) => {
            return (
              <ListGroup.Item key={index}>
                <Link to={`/blog/post/?slug=${item.urlSlug}`} key={index}>
                  {item.fullName}
                </Link>
              </ListGroup.Item>
            );
          })}
        </ListGroup>
      )}
    </div>
  );
};

export default PopularAuthors;
