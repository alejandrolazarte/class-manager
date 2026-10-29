import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { PhotoApprovalCards } from "@/features/userPhotos/components/PhotoApprovalCards";
import { listPhotoApprovals, refusePhoto } from "@/features/userPhotos/userPhotoApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/userPhotos/userPhotoApi");

const approval = {
  id: "photo-1",
  studentFullName: "Tomás Pérez",
  photoUrl: "https://files.example.com/private/photo.webp?sig=abc",
};

describe("When the guardian refuses a photo", () => {
  beforeEach(() => {
    jest.mocked(listPhotoApprovals).mockResolvedValue([approval]);
    jest.mocked(refusePhoto).mockResolvedValue();
  });

  it("Then it is refused", async () => {
    await renderWithProviders(<PhotoApprovalCards />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("photoApproval.refuse") }),
    );
    await screen.findByText(translate("photoApproval.refuseQuestion", { name: "Tomás" }));
    await fireEvent.press(
      screen.getAllByRole("button", { name: translate("photoApproval.confirmRefuse") }).at(-1)!,
    );

    await waitFor(() => expect(refusePhoto).toHaveBeenCalledWith(approval.id));
    expect(
      await screen.findByText(translate("photoApproval.refused", { name: "Tomás" })),
    ).toBeOnTheScreen();
  });
});
