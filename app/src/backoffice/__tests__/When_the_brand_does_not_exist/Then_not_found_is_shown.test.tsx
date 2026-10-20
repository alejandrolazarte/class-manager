import { screen } from "@testing-library/react-native";
import { BackofficeRequestError } from "@/backoffice/api/backofficeHttpClient";
import { BrandScreen } from "@/backoffice/brands/BrandScreen";
import { getBrand } from "@/backoffice/brands/brandsApi";
import { translate } from "@/i18n/translate";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandsApi");

describe("When the brand does not exist", () => {
  beforeEach(() => {
    jest.mocked(getBrand).mockRejectedValue(new BackofficeRequestError(404));
  });

  it("Then not found is shown", async () => {
    await renderBackofficeScreen(<BrandScreen brandId="missing" />);

    expect(await screen.findByText(translate("backoffice.brand.notFound"))).toBeOnTheScreen();
  });
});
