import { PasskeysScreen } from "@/features/passkeys/screens/PasskeysScreen";
import { routes } from "@/navigation/routes";

export default function TodayPasskeysRoute() {
  return <PasskeysScreen addPasskeyRoute={routes.todayAddMyPasskey} />;
}
