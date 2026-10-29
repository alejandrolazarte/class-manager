import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { PhotoApprovalCards } from "@/features/userPhotos/components/PhotoApprovalCards";
import { approvePhoto, listPhotoApprovals, refusePhoto } from "@/features/userPhotos/userPhotoApi";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/userPhotos/userPhotoApi");

const approval = {
  id: "photo-1",
  studentFullName: "Tomás Pérez",
  photoUrl: "https://files.example.com/private/photo.webp?sig=abc",
};

describe("When the guardian approves a photo", () => {
  beforeEach(() => {
    jest.mocked(listPhotoApprovals).mockResolvedValue([approval]);
    jest.mocked(approvePhoto).mockResolvedValue();
  });

  it("Then it is approved", async () => {
    await renderWithProviders(<PhotoApprovalCards />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("photoApproval.approve") }),
    );

    await waitFor(() => expect(approvePhoto).toHaveBeenCalledWith(approval.id));
    expect(
      await screen.findByText(translate("photoApproval.approved", { name: "Tomás" })),
    ).toBeOnTheScreen();
    expect(refusePhoto).not.toHaveBeenCalled();
  });
});
