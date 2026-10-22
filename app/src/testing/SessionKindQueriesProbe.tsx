import { useQuery } from "@tanstack/react-query";
import { Text } from "react-native";
import { RequireSessionKind } from "@/features/authentication/components/RequireSessionKind";

export const teamProbeQueryKey = ["probe", "team"] as const;
export const studentProbeQueryKey = ["probe", "student"] as const;

interface ProbeQueryProps {
  queryKey: readonly string[];
  queryFn: () => Promise<string>;
}

function ProbeQuery({ queryKey, queryFn }: ProbeQueryProps) {
  const { data } = useQuery({ queryKey, queryFn });
  return data === undefined ? null : <Text>{data}</Text>;
}

interface SessionKindQueriesProbeProps {
  loadTeamData: () => Promise<string>;
  loadStudentData: () => Promise<string>;
}

export function SessionKindQueriesProbe({
  loadTeamData,
  loadStudentData,
}: SessionKindQueriesProbeProps) {
  return (
    <>
      <RequireSessionKind kind="team">
        <ProbeQuery queryKey={teamProbeQueryKey} queryFn={loadTeamData} />
      </RequireSessionKind>
      <RequireSessionKind kind="student">
        <ProbeQuery queryKey={studentProbeQueryKey} queryFn={loadStudentData} />
      </RequireSessionKind>
    </>
  );
}
