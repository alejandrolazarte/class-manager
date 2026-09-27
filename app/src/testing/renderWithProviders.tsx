import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { render } from "@testing-library/react-native";
import { ReactElement } from "react";
import { CurrentBusinessProvider } from "@/features/business/CurrentBusinessProvider";
import { Business } from "@/features/business/types";
import { buildBusiness } from "@/testing/businessFactory";
import { createTestQueryClient } from "@/testing/testQueryClients";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { ToastProvider } from "@/ui/ToastProvider";

export { createTestQueryClient };

interface RenderWithProvidersOptions {
  queryClient?: QueryClient;
  business?: Business;
}

export async function renderWithProviders(
  element: ReactElement,
  {
    queryClient = createTestQueryClient(),
    business = buildBusiness(),
  }: RenderWithProvidersOptions = {},
) {
  const renderResult = await render(
    <ThemeProvider initialColorSchemePreference="light">
      <QueryClientProvider client={queryClient}>
        <CurrentBusinessProvider business={business}>
          <ToastProvider>{element}</ToastProvider>
        </CurrentBusinessProvider>
      </QueryClientProvider>
    </ThemeProvider>,
  );
  return { ...renderResult, queryClient };
}
