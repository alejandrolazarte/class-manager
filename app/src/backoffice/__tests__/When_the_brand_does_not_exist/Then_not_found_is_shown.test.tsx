import { screen } from "@testing-library/react-native";
import { BackofficeRequestError } from "@/backoffice/api/backofficeHttpClient";
import { BrandScreen } from "@/backoffice/brands/BrandScreen";
import { getBrand } from "@/backoffice/brands/brandsApi";
import { translate } from "@/i18n/translate";
import { signInToBackofficeAs } from "@/testing/backofficeSession";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandsApi");
jest.mock("@/backoffice/session/backofficeSessionApi");

describe("When the brand does not exist", () => {
  beforeEach(() => {
    signInToBackofficeAs("PlatformReader");
    jest.mocked(getBrand).mockRejectedValue(new BackofficeRequestError(404));
  });

  it("Then not found is shown", async () => {
    await renderBackofficeScreen(<BrandScreen brandId="missing" />);

    expect(await screen.findByText(translate("backoffice.brand.notFound"))).toBeOnTheScreen();
  });
});
