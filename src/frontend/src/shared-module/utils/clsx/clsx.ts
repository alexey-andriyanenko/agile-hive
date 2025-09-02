type Clsx = (...params: Array<string | null | Record<string, boolean | null>>) => string;

export const clsx: Clsx = (...params) => {
  return params
    .map((param) => {
      if (typeof param === "string") {
        return param;
      } else if (param && typeof param === "object") {
        return Object.entries(param)
          .filter(([_, value]) => Boolean(value))
          .map(([key, _]) => key)
          .join(" ");
      }
      return "";
    })
    .filter(Boolean)
    .join(" ");
};
