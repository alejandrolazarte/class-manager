import { PasskeysScreen } from "@/features/passkeys/screens/PasskeysScreen";
import { routes } from "@/navigation/routes";

export default function StudentAppPasskeysRoute() {
  return <PasskeysScreen addPasskeyRoute={routes.studentAppAddPasskey} />;
}
