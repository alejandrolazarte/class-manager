import { fontFamilies } from "@/theme/typography";

const tailwindConfig = jest.requireActual("../../../../tailwind.config.js");

describe("When tailwind config is loaded", () => {
  it("Then every font role has a utility", () => {
    const configuredFontFamilies = tailwindConfig.theme.extend.fontFamily;

    const expectedFontFamilies = Object.fromEntries(
      Object.entries(fontFamilies).map(([role, student]) => [role, [student]]),
    );
    expect(configuredFontFamilies).toEqual(expectedFontFamilies);
  });
});
