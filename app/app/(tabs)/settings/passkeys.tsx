import { PasskeysScreen } from "@/features/passkeys/screens/PasskeysScreen";
import { routes } from "@/navigation/routes";

export default function PasskeysRoute() {
  return <PasskeysScreen addPasskeyRoute={routes.addMyPasskey} />;
}
