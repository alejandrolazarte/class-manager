import { fireEvent, screen } from "@testing-library/react-native";
import { BrandsScreen } from "@/backoffice/brands/BrandsScreen";
import { getBrands } from "@/backoffice/brands/brandsApi";
import { translate } from "@/i18n/translate";
import { aBrandSummary } from "@/testing/backofficeBrands";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandsApi");

describe("When the operator filters by status", () => {
  beforeEach(() => {
    jest
      .mocked(getBrands)
      .mockResolvedValue([
        aBrandSummary(),
        aBrandSummary({ id: "brand-2", name: "Instructora Sur", status: "Ended" }),
      ]);
  });

  it("Then only brands with that status are listed", async () => {
    await renderBackofficeScreen(<BrandsScreen />);
    await screen.findByText("Estudio Centro");

    await fireEvent.press(
      screen.getByRole("button", { name: translate("backoffice.brands.filters.status") }),
    );
    await fireEvent.press(
      screen.getByRole("radio", { name: translate("backoffice.brands.status.Ended") }),
    );

    expect(screen.getByText("Instructora Sur")).toBeOnTheScreen();
    expect(screen.queryByText("Estudio Centro")).not.toBeOnTheScreen();
  });
});
