import { render, screen } from "@testing-library/react-native";
import { AppText, textToneClassNames } from "@/ui/AppText";

describe("When text has no tone", () => {
  it("Then foreground tone is applied", async () => {
    await render(<AppText className="mt-2">Hola</AppText>);

    expect(screen.getByText("Hola")).toHaveProp(
      "className",
      expect.stringContaining(textToneClassNames.default),
    );
  });
});
