import { Redirect } from "expo-router";
import { PropsWithChildren } from "react";
import { SessionKind } from "@/features/authentication/sessionKind";
import { useSession } from "@/features/authentication/useSession";
import { routes } from "@/navigation/routes";
import { LoadingScreen } from "@/ui/LoadingScreen";

interface RequireSessionKindProps extends PropsWithChildren {
  kind: SessionKind;
}

export function homeRouteOf(kind: SessionKind): string {
  return kind === "student" ? routes.studentApp : routes.today;
}

export function RequireSessionKind({ kind, children }: RequireSessionKindProps) {
  const { session } = useSession();
  if (session.status === "restoring") {
    return <LoadingScreen />;
  }
  if (session.status === "signedOut") {
    return <Redirect href={routes.welcome} />;
  }
  if (session.kind !== kind) {
    return <Redirect href={homeRouteOf(session.kind)} />;
  }
  return children;
}
