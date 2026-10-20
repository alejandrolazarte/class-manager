import { Redirect } from "expo-router";
import { PropsWithChildren } from "react";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";
import { useBackofficeSession } from "@/backoffice/session/useBackofficeSession";
import { SkeletonScreen } from "@/ui/SkeletonScreen";

export function BackofficeSessionGate({ children }: PropsWithChildren) {
  const { session } = useBackofficeSession();

  if (session.isPending) {
    return <SkeletonScreen />;
  }
  if (session.data === null) {
    return <Redirect href={backofficeRoutes.signIn} />;
  }
  return children;
}
