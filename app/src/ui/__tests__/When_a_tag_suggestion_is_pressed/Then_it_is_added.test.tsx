import { fireEvent, render, screen } from "@testing-library/react-native";
import { ThemeProvider } from "@/theme/ThemeProvider";
import { TagInput } from "@/ui/TagInput";

function renderTagInput(values: string[], onChange: (values: string[]) => void) {
  return render(
    <ThemeProvider initialColorSchemePreference="light">
      <TagInput
        label="Talles"
        values={values}
        onChange={onChange}
        addLabel="Agregar"
        removeLabelOf={(value) => `Quitar ${value}`}
        duplicateMessageOf={(value) => `Ya agregaste ${value}`}
        suggestions={["XS", "XL", "XXL"]}
        presets={[{ label: "XS a XL", values: ["XS", "S", "M", "L", "XL"] }]}
      />
    </ThemeProvider>,
  );
}

describe("When a tag suggestion is pressed", () => {
  it("Then it is added", async () => {
    const onChange = jest.fn();
    await renderTagInput(["XS"], onChange);

    await fireEvent.changeText(screen.getByLabelText("Talles"), "x");
    await fireEvent.press(screen.getByRole("button", { name: "XXL" }));

    expect(onChange).toHaveBeenCalledWith(["XS", "XXL"]);
  });
});
