import { Redirect } from "expo-router";
import { PropsWithChildren } from "react";
import { homeRouteOf } from "@/features/authentication/components/RequireSessionKind";
import { useSession } from "@/features/authentication/useSession";
import { routes } from "@/navigation/routes";
import { LoadingScreen } from "@/ui/LoadingScreen";

export function RedirectWhenSignedIn({ children }: PropsWithChildren) {
  const { session } = useSession();
  if (session.status === "restoring") {
    return <LoadingScreen />;
  }
  if (session.status === "signedIn") {
    return (
      <Redirect
        href={session.mustChooseAccount ? routes.chooseAccount : homeRouteOf(session.kind)}
      />
    );
  }
  return children;
}
