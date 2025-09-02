export const pickColor = (name: string) => {
  const colorPalette = ["red", "blue", "green", "yellow", "purple", "orange"];
  const index = name.charCodeAt(0) % colorPalette.length;
  return colorPalette[index];
};
