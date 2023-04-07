import { useEffect, useState } from "react";
import ListGroup from "react-bootstrap/ListGroup";
import { Link } from "react-router-dom";
import { getTagCloud } from "../../Services/Widgets";

const TagCloudWidget = () => {
  const [tagList, setTagList] = useState([]);

  useEffect(() => {
    getTagCloud().then((data) => {
      if (data) {
        setTagList(data.items);
      } else setTagList([]);
    });
  }, []);

  return (
    <div className="mb-4">
      <h3 className="text-success mb-2">Các chủ đề</h3>
      {tagList.length > 0 && (
        <ListGroup>
          {tagList.map((item, index) => {
            if (item.postCount > 0) {
              return (
                <ListGroup.Item key={index}>
                  <Link
                    key={index}
                    to={`/blog/tag/slug=${item.urlSlug}`}
                    className="btn btn-sm btn-outline-secondary me-2 mb-2"
                  >
                    {item.name} &nbsp;({item.postCount})
                  </Link>
                </ListGroup.Item>
              );
            } else return <></>;
          })}
        </ListGroup>
      )}
    </div>
  );
};

export default TagCloudWidget;
