import { screen } from "@testing-library/react-native";
import { getBrand, getBrandLogo } from "@/features/brand/brandApi";
import { BrandProvider } from "@/features/brand/BrandProvider";
import { CurrentBrandLogo } from "@/features/brand/components/CurrentBrandLogo";
import { brandLogoDataUri, buildBrand } from "@/testing/brandFactory";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/brand/brandApi");

describe("When brand has a logo", () => {
  beforeEach(() => {
    jest.mocked(getBrand).mockResolvedValue(buildBrand({ logoUpdatedAt: "2026-09-30T12:00:00Z" }));
    jest.mocked(getBrandLogo).mockResolvedValue(brandLogoDataUri);
  });

  it("Then it is shown in the header", async () => {
    await renderWithSession(
      <BrandProvider audience="student">
        <CurrentBrandLogo />
      </BrandProvider>,
    );

    const logoImages = await screen.findAllByTestId("brand-logo-image");
    expect(logoImages[0]!.props.source).toEqual({ uri: brandLogoDataUri });
  });
});
