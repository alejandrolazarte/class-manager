export const studentQueryKeys = {
  all: ["students"] as const,
  search: (search: string) => [...studentQueryKeys.all, "search", search] as const,
};
