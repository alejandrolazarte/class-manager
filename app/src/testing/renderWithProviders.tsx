import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { render } from "@testing-library/react-native";
import { ReactElement } from "react";
import { CurrentBusinessProvider } from "@/features/business/CurrentBusinessProvider";
import { Business } from "@/features/business/types";
import { CurrentMemberProvider } from "@/features/members/CurrentMemberProvider";
import { CurrentMember } from "@/features/members/types";
import { PlanOfferProvider } from "@/features/subscriptions/PlanOfferProvider";
import { defaultLanguage } from "@/i18n/languages";
import { LanguageProvider } from "@/i18n/LanguageProvider";
import { buildBusiness } from "@/testing/businessFactory";
import { buildCurrentMember } from "@/testing/memberFactory";
import { createTestQueryClient } from "@/testing/testQueryClients";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { ToastProvider } from "@/ui/ToastProvider";

export { createTestQueryClient };

interface RenderWithProvidersOptions {
  queryClient?: QueryClient;
  business?: Business;
  member?: CurrentMember;
}

export async function renderWithProviders(
  element: ReactElement,
  {
    queryClient = createTestQueryClient(),
    business = buildBusiness(),
    member = buildCurrentMember(),
  }: RenderWithProvidersOptions = {},
) {
  const renderResult = await render(
    <ThemeProvider initialColorSchemePreference="light">
      <QueryClientProvider client={queryClient}>
        <CurrentBusinessProvider business={business}>
          <CurrentMemberProvider member={member}>
            <ToastProvider>
              <LanguageProvider initialLanguage={defaultLanguage}>
                <PlanOfferProvider>{element}</PlanOfferProvider>
              </LanguageProvider>
            </ToastProvider>
          </CurrentMemberProvider>
        </CurrentBusinessProvider>
      </QueryClientProvider>
    </ThemeProvider>,
  );
  return { ...renderResult, queryClient };
}
