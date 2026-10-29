import { screen } from "@testing-library/react-native";
import { getStudentAppFamily } from "@/features/studentApp/studentAppApi";
import { StudentAppFamilyScreen } from "@/features/studentApp/screens/StudentAppFamilyScreen";
import { buildStudentAppFamily } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const photoUrl = "https://files.example.com/private/tomas.webp?sig=abc";

describe("When a family member has a photo", () => {
  beforeEach(() => {
    const family = buildStudentAppFamily();
    jest.mocked(getStudentAppFamily).mockResolvedValue({
      members: family.members.map((member) =>
        member.paysForFamily ? member : { ...member, photoUrl },
      ),
    });
  });

  it("Then the family shows it", async () => {
    await renderStudentAppScreen(<StudentAppFamilyScreen />);

    await screen.findByText("ana@example.com");
    expect(screen.getByTestId("avatar-photo", { includeHiddenElements: true })).toHaveProp(
      "source",
      { uri: photoUrl },
    );
  });
});
