import React from "react";
import SearchForm from "./SearchForm";
import CategoriesWidget from "./widgets/CategoriesWidget";
import FeaturedPostsWidget from "./widgets/FeaturedPostsWidget";
import RandomPostsWidget from "./widgets/RandomPostsWidget";
import TagCloudWidget from "./widgets/TagWidget";
import PopularAuthors from "./widgets/BestAuthorWidget";
import ArchiveWidget from "./widgets/ArchiveWidget";

const Sidebar = () => {
  return (
    <div className="mb-4 pt-4 ps-2">
      <SearchForm />
      <CategoriesWidget />
      <FeaturedPostsWidget />
      <RandomPostsWidget />
      <TagCloudWidget />
      <PopularAuthors />
      <ArchiveWidget />
    </div>
  );
};

export default Sidebar;
