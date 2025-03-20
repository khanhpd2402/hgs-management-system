export const formatDate = (date) => {
  if (!date) return null;
  return date.toISOString().split("T")[0]; // Gets YYYY-MM-DD format
};
