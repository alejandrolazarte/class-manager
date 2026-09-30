import { Redirect } from "expo-router";
import { ChooseAccountScreen } from "@/features/authentication/screens/ChooseAccountScreen";
import { useSession } from "@/features/authentication/useSession";
import { routes } from "@/navigation/routes";
import { LoadingScreen } from "@/ui/LoadingScreen";

export default function ChooseAccountRoute() {
  const { session } = useSession();
  if (session.status === "restoring") {
    return <LoadingScreen />;
  }
  if (session.status === "signedOut") {
    return <Redirect href={routes.welcome} />;
  }
  return <ChooseAccountScreen />;
}
