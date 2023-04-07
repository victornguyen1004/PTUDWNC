import { Link } from "react-router-dom";

const TagsList = ({ tagsList }) => {
  if (tagsList && Array.isArray(tagsList) && tagsList.length > 0)
    return (
      <>
        {tagsList.map((item, index) => {
          return (
            <Link
              to={`/blog/tag?slug=${item.urlSlug}`}
              title={item.name}
              className="btn btn-sm btn-outline-secondary me-1"
              key={index}
            >
              {item.name}
            </Link>
          );
        })}
      </>
    );
  else return <></>;
};

export default TagsList;
