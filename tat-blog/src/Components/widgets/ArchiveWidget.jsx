import { useEffect, useState } from "react";
import ListGroup from "react-bootstrap/ListGroup";
import { Link } from "react-router-dom";
import { getArchives } from "../../Services/Widgets";
import { getMonthName } from "../../Utils/Utils";

const ArchiveWidget = () => {
  const [posts, setPosts] = useState([]);

  useEffect(() => {
    getArchives(12).then((data) => {
      if (data) {
        setPosts(data);
      } else setPosts([]);
    });
  }, []);
  return (
    <div className="mb-4">
      <h3 className="text-success mb-2 text-success">Bài viết theo tháng</h3>
      {posts.length > 0 && (
        <ListGroup>
          {posts.map((item, index) => {
            return (
              <ListGroup.Item key={index}>
                <Link
                  to={`/blog/archive/${item.year}/${item.month}`}
                  key={index}
                >
                  {`${getMonthName(item.month)} ${item.year} (${
                    item.postsCount
                  })`}
                </Link>
              </ListGroup.Item>
            );
          })}
        </ListGroup>
      )}
    </div>
  );
};

export default ArchiveWidget;
