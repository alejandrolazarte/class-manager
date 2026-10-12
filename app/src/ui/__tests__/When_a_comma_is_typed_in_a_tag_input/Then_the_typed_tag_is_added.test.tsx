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

describe("When a comma is typed in a tag input", () => {
  it("Then the typed tag is added", async () => {
    const onChange = jest.fn();
    await renderTagInput([], onChange);

    await fireEvent.changeText(screen.getByLabelText("Talles"), "38,");

    expect(onChange).toHaveBeenCalledWith(["38"]);
  });
});
