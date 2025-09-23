export const toSlug = (input: string) => {
  let slug = input.trim().toLowerCase().replace(/ /g, "-"); // Replace spaces with hyphens

  slug = slug.replace(/[^a-z0-9-]/g, ""); // Remove invalid characters
  slug = slug.replace(/-{2,}/g, "-"); // Replace multiple hyphens with one

  return slug;
};
