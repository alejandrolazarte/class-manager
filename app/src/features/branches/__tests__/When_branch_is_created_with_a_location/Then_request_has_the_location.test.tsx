import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { createBranch } from "@/features/branches/branchesApi";
import { NewBranchScreen } from "@/features/branches/screens/NewBranchScreen";
import { translate } from "@/i18n/translate";
import { buildBranch } from "@/testing/branchFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/branches/branchesApi");
jest.mock("@/features/authentication/useSession", () => ({
  useSession: () => ({ switchBranch: jest.fn() }),
}));

const locationName = "Piscina Alboraya, Valencia";
const locationMapsUrl = "https://maps.app.goo.gl/r7Tn2pQx9L";

describe("When branch is created with a location", () => {
  beforeEach(() => {
    jest.mocked(createBranch).mockResolvedValue(buildBranch({ name: "DF Valencia" }));
  });

  it("Then request has the location", async () => {
    await renderWithProviders(<NewBranchScreen />);
    await fireEvent.changeText(
      screen.getByLabelText(translate("branches.form.name")),
      "DF Valencia",
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("businessLocation.link")),
      locationMapsUrl,
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("businessLocation.name")),
      locationName,
    );

    await fireEvent.press(screen.getByRole("button", { name: translate("branches.form.submit") }));

    await waitFor(() =>
      expect(createBranch).toHaveBeenCalledWith(
        expect.objectContaining({ locationName, locationMapsUrl }),
      ),
    );
  });
});
