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

describe("When a tag preset is pressed", () => {
  it("Then all its values are added", async () => {
    const onChange = jest.fn();
    await renderTagInput([], onChange);

    await fireEvent.press(screen.getByRole("button", { name: "XS a XL" }));

    expect(onChange).toHaveBeenCalledWith(["XS", "S", "M", "L", "XL"]);
  });
});
