import { screen } from "@testing-library/react-native";
import { FeatureList } from "@/features/subscriptions/components/FeatureList";
import { renderWithProviders } from "@/testing/renderWithProviders";

describe("When the plan includes document storage", () => {
  it("Then it reads in gigabytes", async () => {
    await renderWithProviders(
      <FeatureList features={[{ code: "document-storage", limit: 1024 }]} />,
    );

    expect(screen.getByText("Espacio para archivos: hasta 1 GB")).toBeTruthy();
  });
});
