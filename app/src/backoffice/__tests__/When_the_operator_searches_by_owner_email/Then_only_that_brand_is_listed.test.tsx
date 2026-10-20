import { fireEvent, screen } from "@testing-library/react-native";
import { BrandsScreen } from "@/backoffice/brands/BrandsScreen";
import { getBrands } from "@/backoffice/brands/brandsApi";
import { translate } from "@/i18n/translate";
import { aBrandSummary } from "@/testing/backofficeBrands";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandsApi");

describe("When the operator searches by owner email", () => {
  beforeEach(() => {
    jest
      .mocked(getBrands)
      .mockResolvedValue([
        aBrandSummary(),
        aBrandSummary({ id: "brand-2", name: "Instructora Sur", ownerEmail: "sol@example.com" }),
      ]);
  });

  it("Then only that brand is listed", async () => {
    await renderBackofficeScreen(<BrandsScreen />);
    await screen.findByText("Estudio Centro");

    await fireEvent.changeText(
      screen.getByPlaceholderText(translate("backoffice.brands.searchPlaceholder")),
      "SOL@example",
    );

    expect(screen.getByText("Instructora Sur")).toBeOnTheScreen();
    expect(screen.queryByText("Estudio Centro")).not.toBeOnTheScreen();
  });
});
