import { Redirect } from "expo-router";
import { PropsWithChildren } from "react";
import { useSession } from "@/features/authentication/useSession";
import { routes } from "@/navigation/routes";
import { LoadingScreen } from "@/ui/LoadingScreen";

export function RedirectWhenSignedOut({ children }: PropsWithChildren) {
  const { session } = useSession();
  if (session.status === "restoring") {
    return <LoadingScreen />;
  }
  if (session.status === "signedOut") {
    return <Redirect href={routes.signIn} />;
  }
  return children;
}
