import { typeScale } from "@/theme/typography";

const tailwindConfig = jest.requireActual("../../../../tailwind.config.js");

describe("When tailwind config is loaded", () => {
  it("Then every type scale size has a utility", () => {
    const configuredFontSizes = tailwindConfig.theme.fontSize;

    const expectedFontSizes = Object.fromEntries(
      Object.entries(typeScale).map(([size, { fontSize, lineHeight }]) => [
        size,
        [`${fontSize}px`, `${lineHeight}px`],
      ]),
    );
    expect(configuredFontSizes).toEqual(expectedFontSizes);
  });
});
