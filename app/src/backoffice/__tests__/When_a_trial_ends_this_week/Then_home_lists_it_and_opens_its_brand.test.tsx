import { fireEvent, screen } from "@testing-library/react-native";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";
import { getBrands } from "@/backoffice/brands/brandsApi";
import { getExpiringAddOns } from "@/backoffice/brands/expiringAddOnsApi";
import { BackofficeHomeScreen } from "@/backoffice/home/BackofficeHomeScreen";
import { aBrandSummary } from "@/testing/backofficeBrands";
import { signInToBackofficeAs } from "@/testing/backofficeSession";
import { routerMock } from "@/testing/expoRouterMock";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/session/backofficeSessionApi");
jest.mock("@/backoffice/brands/brandsApi");
jest.mock("@/backoffice/brands/expiringAddOnsApi");

describe("When a trial ends this week", () => {
  beforeEach(() => {
    signInToBackofficeAs("PlatformReader");
    jest.mocked(getBrands).mockResolvedValue([
      aBrandSummary(),
      aBrandSummary({
        id: "brand-2",
        name: "Instructora Sur",
        planCode: "free",
        status: "Trial",
        trialDaysLeft: 3,
      }),
      aBrandSummary({
        id: "brand-3",
        name: "Estudio Nuevo",
        planCode: "free",
        status: "Trial",
        trialDaysLeft: 25,
      }),
    ]);
    jest.mocked(getExpiringAddOns).mockResolvedValue([]);
  });

  it("Then home lists it and opens its brand", async () => {
    await renderBackofficeScreen(<BackofficeHomeScreen />);

    await fireEvent.press(await screen.findByRole("button", { name: "Instructora Sur" }));

    expect(screen.queryByRole("button", { name: "Estudio Nuevo" })).not.toBeOnTheScreen();
    expect(routerMock.push).toHaveBeenCalledWith(backofficeRoutes.brand("brand-2"));
  });
});
