import { screen } from "@testing-library/react-native";
import { getBrand } from "@/features/brand/brandApi";
import { brandInitials } from "@/features/brand/brandInitials";
import { BrandProvider } from "@/features/brand/BrandProvider";
import { translate } from "@/i18n/translate";
import { buildBrand } from "@/testing/brandFactory";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/brand/brandApi");

const brand = buildBrand({ logoUpdatedAt: null });

describe("When brand has no logo", () => {
  beforeEach(() => {
    jest.mocked(getBrand).mockResolvedValue(brand);
  });

  it("Then its initials are shown", async () => {
    await renderWithSession(<BrandProvider audience="student">{null}</BrandProvider>);

    expect(await screen.findByText(brandInitials(brand.displayName))).toBeTruthy();
    expect(screen.queryByRole("progressbar", { name: translate("common.loading") })).toBeNull();
  });
});
