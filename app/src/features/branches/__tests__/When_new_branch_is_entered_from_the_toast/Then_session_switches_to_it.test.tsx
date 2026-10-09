import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createBranch } from "@/features/branches/branchesApi";
import { NewBranchScreen } from "@/features/branches/screens/NewBranchScreen";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildBranch } from "@/testing/branchFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

const mockSwitchBranch = jest.fn();

jest.mock("@/features/branches/branchesApi");
jest.mock("@/features/authentication/useSession", () => ({
  useSession: () => ({ switchBranch: mockSwitchBranch }),
}));

const newBranch = buildBranch({ businessId: "branch-new", name: "Sede Norte", isCurrent: false });

describe("When new branch is entered from the toast", () => {
  beforeEach(() => {
    mockSwitchBranch.mockReset().mockResolvedValue(undefined);
    jest.mocked(createBranch).mockResolvedValue(newBranch);
  });

  it("Then session switches to it", async () => {
    await renderWithProviders(<NewBranchScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("branches.form.name")),
      newBranch.name,
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("branches.form.submit") }));

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("branches.form.enter") }),
    );

    await waitFor(() => expect(routerMock.replace).toHaveBeenCalledWith(routes.today));
    expect(mockSwitchBranch).toHaveBeenCalledWith(newBranch.businessId);
  });
});
