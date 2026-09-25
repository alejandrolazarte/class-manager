import { useQuery } from "@tanstack/react-query";
import { classGroupQueryKeys } from "@/features/classGroups/classGroupQueryKeys";
import {
  listActiveClassGroups,
  listClassGroupsIncludingInactive,
} from "@/features/classGroups/classGroupsApi";

export function useActiveClassGroups() {
  return useQuery({ queryKey: classGroupQueryKeys.active, queryFn: listActiveClassGroups });
}

export function useClassGroupsIncludingInactive() {
  return useQuery({
    queryKey: classGroupQueryKeys.includingInactive,
    queryFn: listClassGroupsIncludingInactive,
  });
}
