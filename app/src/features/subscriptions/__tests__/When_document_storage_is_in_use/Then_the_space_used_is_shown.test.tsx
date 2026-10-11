import { screen } from "@testing-library/react-native";
import { FeatureList } from "@/features/subscriptions/components/FeatureList";
import { renderWithProviders } from "@/testing/renderWithProviders";

describe("When document storage is in use", () => {
  it("Then the space used is shown", async () => {
    await renderWithProviders(
      <FeatureList features={[{ code: "document-storage", limit: 5120, used: 230 }]} />,
    );

    expect(screen.getByText("Espacio para archivos: 230 MB de 5 GB")).toBeTruthy();
  });
});
