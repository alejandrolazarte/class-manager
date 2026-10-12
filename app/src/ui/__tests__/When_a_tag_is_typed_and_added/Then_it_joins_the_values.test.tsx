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

describe("When a tag is typed and added", () => {
  it("Then it joins the values", async () => {
    const onChange = jest.fn();
    await renderTagInput(["S"], onChange);

    await fireEvent.changeText(screen.getByLabelText("Talles"), " M ");
    await fireEvent.press(screen.getByRole("button", { name: "Agregar" }));

    expect(onChange).toHaveBeenCalledWith(["S", "M"]);
  });
});
