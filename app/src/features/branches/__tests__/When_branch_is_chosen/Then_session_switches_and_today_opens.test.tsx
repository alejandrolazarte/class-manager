import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listBranches } from "@/features/branches/branchesApi";
import { BranchesScreen } from "@/features/branches/screens/BranchesScreen";
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

const currentBranch = buildBranch();
const otherBranch = buildBranch({
  businessId: "branch-valencia",
  name: "DF Valencia",
  branchRole: null,
  isCurrent: false,
});

describe("When branch is chosen", () => {
  beforeEach(() => {
    mockSwitchBranch.mockReset().mockResolvedValue(undefined);
    jest.mocked(listBranches).mockResolvedValue([currentBranch, otherBranch]);
  });

  it("Then session switches and today opens", async () => {
    await renderWithProviders(<BranchesScreen />);
    expect(await screen.findByText(translate("branches.current"))).toBeOnTheScreen();

    await fireEvent.press(screen.getByRole("button", { name: otherBranch.name }));

    await waitFor(() => expect(routerMock.replace).toHaveBeenCalledWith(routes.today));
    expect(mockSwitchBranch).toHaveBeenCalledWith(otherBranch.businessId);
  });
});
