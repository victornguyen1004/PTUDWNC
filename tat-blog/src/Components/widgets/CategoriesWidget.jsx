import { useEffect, useState } from "react";
import ListGroup from "react-bootstrap/ListGroup";
import { Link } from "react-router-dom";
import { getCategories } from "../../Services/Widgets";

const CategoriesWidget = () => {
  const [categoriesList, setCategoriesList] = useState([]);

  useEffect(() => {
    getCategories().then((data) => {
      if (data) {
        setCategoriesList(data.items);
      } else setCategoriesList([]);
    });
  }, []);

  return (
    <div className="mb-4">
      <h3 className="text-success mb-2">Các chủ đề</h3>
      {categoriesList.length > 0 && (
        <ListGroup>
          {categoriesList.map((item, index) => {
            return (
              <ListGroup.Item key={index}>
                <Link
                  to={`/blog/category?slug=${item.urlSlug}`}
                  title={item.description}
                  key={index}
                >
                  {item.name}
                  <span>&nbsp;({item.postCount})</span>
                </Link>
              </ListGroup.Item>
            );
          })}
        </ListGroup>
      )}
    </div>
  );
};

export default CategoriesWidget;
