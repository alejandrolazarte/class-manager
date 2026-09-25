import "@/global.css";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { SplashScreen, Stack } from "expo-router";
import { StatusBar } from "expo-status-bar";
import { useEffect, useState } from "react";
import { SafeAreaProvider } from "react-native-safe-area-context";
import { SessionProvider } from "@/features/authentication/SessionProvider";
import { useSession } from "@/features/authentication/useSession";
import { ToastProvider } from "@/ui/ToastProvider";

SplashScreen.preventAutoHideAsync().catch(() => undefined);

function RootNavigator() {
  const { session } = useSession();
  const isRestoringSession = session.status === "restoring";

  useEffect(() => {
    if (!isRestoringSession) {
      SplashScreen.hideAsync().catch(() => undefined);
    }
  }, [isRestoringSession]);

  return <Stack screenOptions={{ headerShown: false }} />;
}

export default function RootLayout() {
  const [queryClient] = useState(() => new QueryClient());
  return (
    <SafeAreaProvider>
      <QueryClientProvider client={queryClient}>
        <SessionProvider>
          <ToastProvider>
            <StatusBar style="dark" />
            <RootNavigator />
          </ToastProvider>
        </SessionProvider>
      </QueryClientProvider>
    </SafeAreaProvider>
  );
}
