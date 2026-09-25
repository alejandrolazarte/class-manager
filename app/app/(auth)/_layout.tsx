import { Stack } from "expo-router";
import { RedirectWhenSignedIn } from "@/features/authentication/components/RedirectWhenSignedIn";

export default function AuthenticationLayout() {
  return (
    <RedirectWhenSignedIn>
      <Stack screenOptions={{ headerShown: false }} />
    </RedirectWhenSignedIn>
  );
}
