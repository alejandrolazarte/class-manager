import { Stack } from "expo-router";
import { RedirectWhenSignedIn } from "@/features/authentication/components/RedirectWhenSignedIn";
import { useStackScreenOptions } from "@/navigation/stackScreenOptions";

export default function AuthenticationLayout() {
  const stackScreenOptions = useStackScreenOptions();
  return (
    <RedirectWhenSignedIn>
      <Stack screenOptions={stackScreenOptions} />
    </RedirectWhenSignedIn>
  );
}
