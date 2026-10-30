import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { BrandScreen } from "@/backoffice/brands/BrandScreen";
import { getBrand } from "@/backoffice/brands/brandsApi";
import { copyText } from "@/backoffice/clipboard";
import { translate } from "@/i18n/translate";
import { aBrandDetail, aPlanOffer } from "@/testing/backofficeBrands";
import { signInToBackofficeAs } from "@/testing/backofficeSession";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandsApi");
jest.mock("@/backoffice/clipboard");
jest.mock("@/backoffice/session/backofficeSessionApi");

describe("When the operator copies the payment link", () => {
  beforeEach(() => {
    signInToBackofficeAs("PlatformOperator");
    jest.mocked(getBrand).mockResolvedValue(aBrandDetail({ offer: aPlanOffer() }));
    jest.mocked(copyText).mockResolvedValue();
  });

  it("Then the link to the app is copied", async () => {
    await renderBackofficeScreen(<BrandScreen brandId="brand-1" />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("backoffice.brand.copyPaymentLink") }),
    );

    await waitFor(() => expect(copyText).toHaveBeenCalledWith(aPlanOffer().paymentLink));
    expect(await screen.findByText(translate("backoffice.brand.paymentLinkCopied"))).toBeTruthy();
  });
});
