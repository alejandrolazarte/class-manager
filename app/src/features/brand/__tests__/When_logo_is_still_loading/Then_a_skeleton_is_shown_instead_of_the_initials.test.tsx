import { screen } from "@testing-library/react-native";
import { getBrand, getBrandLogo } from "@/features/brand/brandApi";
import { brandInitials } from "@/features/brand/brandInitials";
import { BrandProvider } from "@/features/brand/BrandProvider";
import { translate } from "@/i18n/translate";
import { buildBrand } from "@/testing/brandFactory";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/brand/brandApi");

const brand = buildBrand({ logoUpdatedAt: "2026-09-30T12:00:00Z" });

describe("When logo is still loading", () => {
  beforeEach(() => {
    jest.mocked(getBrand).mockResolvedValue(brand);
    jest.mocked(getBrandLogo).mockReturnValue(new Promise<string>(() => {}));
  });

  it("Then a skeleton is shown instead of the initials", async () => {
    await renderWithSession(<BrandProvider audience="student">{null}</BrandProvider>);

    expect(await screen.findByText(brand.displayName)).toBeTruthy();
    expect(screen.getByRole("progressbar", { name: translate("common.loading") })).toBeTruthy();
    expect(screen.queryByText(brandInitials(brand.displayName))).toBeNull();
  });
});
