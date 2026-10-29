import { fireEvent, screen } from "@testing-library/react-native";
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

describe("When the guardian does not confirm refusing a photo", () => {
  beforeEach(() => {
    jest.mocked(listPhotoApprovals).mockResolvedValue([approval]);
  });

  it("Then nothing is refused", async () => {
    await renderWithProviders(<PhotoApprovalCards />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("photoApproval.refuse") }),
    );
    expect(
      await screen.findByText(translate("photoApproval.refuseQuestion", { name: "Tomás" })),
    ).toBeOnTheScreen();
    await fireEvent.press(
      screen.getAllByRole("button", { name: translate("common.back") }).at(-1)!,
    );

    expect(refusePhoto).not.toHaveBeenCalled();
    expect(approvePhoto).not.toHaveBeenCalled();
  });
});
