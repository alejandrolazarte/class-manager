import { waitFor } from "@testing-library/react-native";
import { BackofficeSessionEndedError } from "@/backoffice/api/backofficeHttpClient";
import { backofficeRoutes } from "@/backoffice/backofficeRoutes";
import { BrandsScreen } from "@/backoffice/brands/BrandsScreen";
import { getBrands } from "@/backoffice/brands/brandsApi";
import { routerMock } from "@/testing/expoRouterMock";
import { renderBackofficeScreen } from "@/testing/renderBackofficeScreen";

jest.mock("@/backoffice/brands/brandsApi");

describe("When the session ended while listing brands", () => {
  beforeEach(() => {
    jest.mocked(getBrands).mockRejectedValue(new BackofficeSessionEndedError());
  });

  it("Then sign in opens", async () => {
    await renderBackofficeScreen(<BrandsScreen />);

    await waitFor(() => expect(routerMock.replace).toHaveBeenCalledWith(backofficeRoutes.signIn));
  });
});
