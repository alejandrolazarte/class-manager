import { QueryClientProvider } from "@tanstack/react-query";
import { render } from "@testing-library/react-native";
import { ReactElement } from "react";
import { createTestQueryClient } from "@/testing/testQueryClients";
import { ThemeProvider } from "@/theme/ThemeProvider";

export async function renderBackofficeScreen(element: ReactElement) {
  return render(
    <QueryClientProvider client={createTestQueryClient()}>
      <ThemeProvider>{element}</ThemeProvider>
    </QueryClientProvider>,
  );
}
