import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createBranch } from "@/features/branches/branchesApi";
import { NewBranchScreen } from "@/features/branches/screens/NewBranchScreen";
import { translate } from "@/i18n/translate";
import { buildBusiness } from "@/testing/businessFactory";
import { buildBranch } from "@/testing/branchFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/branches/branchesApi");

const madridBusiness = buildBusiness({
  timeZoneId: "Europe/Madrid",
  currencyCode: "EUR",
  defaultCountryCallingCode: "34",
});
const branchName = "DF Valencia";

describe("When branch is created", () => {
  beforeEach(() => {
    jest
      .mocked(createBranch)
      .mockResolvedValue(buildBranch({ name: branchName, isCurrent: false }));
  });

  it("Then request has name and regional settings", async () => {
    await renderWithProviders(<NewBranchScreen />, { business: madridBusiness });
    await fireEvent.changeText(screen.getByLabelText(translate("branches.form.name")), branchName);

    await fireEvent.press(screen.getByRole("button", { name: translate("branches.form.submit") }));

    await waitFor(() =>
      expect(createBranch).toHaveBeenCalledWith({
        name: branchName,
        timeZoneId: "Europe/Madrid",
        currencyCode: "EUR",
        defaultCountryCallingCode: "34",
      }),
    );
    expect(routerMock.back).toHaveBeenCalled();
  });
});
