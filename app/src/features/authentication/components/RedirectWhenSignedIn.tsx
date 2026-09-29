import { Redirect } from "expo-router";
import { PropsWithChildren } from "react";
import { homeRouteOf } from "@/features/authentication/components/RequireSessionKind";
import { useSession } from "@/features/authentication/useSession";
import { LoadingScreen } from "@/ui/LoadingScreen";

export function RedirectWhenSignedIn({ children }: PropsWithChildren) {
  const { session } = useSession();
  if (session.status === "restoring") {
    return <LoadingScreen />;
  }
  if (session.status === "signedIn") {
    return <Redirect href={homeRouteOf(session.kind)} />;
  }
  return children;
}
