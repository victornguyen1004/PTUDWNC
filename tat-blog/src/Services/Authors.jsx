import axios from "axios";

export async function getAuthorsBySlug(slug = "") {
  try {
    const response = await axios.get(
      `https://localhost:7298/api/authors?slug=${slug}}`
    );

    const data = response.data;
    if (data.isSuccess) {
      return data.result;
    } else return null;
  } catch (error) {
    console.log("Error ", error.message);
    return null;
  }
}
