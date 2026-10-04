import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getBrand } from "@/features/brand/brandApi";
import { BrandProvider } from "@/features/brand/BrandProvider";
import { translate } from "@/i18n/translate";
import { buildBrand } from "@/testing/brandFactory";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/brand/brandApi");

const brand = buildBrand({ displayName: "Club Delta", themeColor: "#5b3fa0" });

describe("When app opens with a brand", () => {
  beforeEach(() => {
    jest.mocked(getBrand).mockResolvedValue(brand);
  });

  it("Then the welcome shows it", async () => {
    await renderWithSession(<BrandProvider audience="student">{null}</BrandProvider>);

    expect(await screen.findByText(brand.displayName)).toBeTruthy();
    expect(screen.getByText(translate("brand.welcome.message"))).toBeTruthy();
    await fireEvent.press(screen.getByRole("button", { name: translate("brand.welcome.skip") }));
    await waitFor(() => expect(screen.queryByText(translate("brand.welcome.message"))).toBeNull());
  });
});
