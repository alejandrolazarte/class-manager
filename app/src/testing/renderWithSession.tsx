import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { render } from "@testing-library/react-native";
import { ReactElement } from "react";
import { SessionProvider } from "@/features/authentication/SessionProvider";
import { createTestQueryClient } from "@/testing/testQueryClients";
import { ToastProvider } from "@/ui/ToastProvider";

interface RenderWithSessionOptions {
  queryClient?: QueryClient;
}

export async function renderWithSession(
  element: ReactElement,
  { queryClient = createTestQueryClient() }: RenderWithSessionOptions = {},
) {
  const renderResult = await render(
    <QueryClientProvider client={queryClient}>
      <SessionProvider>
        <ToastProvider>{element}</ToastProvider>
      </SessionProvider>
    </QueryClientProvider>,
  );
  return { ...renderResult, queryClient };
}
