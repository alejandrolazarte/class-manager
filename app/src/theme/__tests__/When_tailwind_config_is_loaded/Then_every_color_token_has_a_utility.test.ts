import { themeColorTokens } from "@/theme/themeColorTokens";

const tailwindConfig = jest.requireActual("../../../../tailwind.config.js");

describe("When tailwind config is loaded", () => {
  it("Then every color token has a utility", () => {
    const configuredColors = tailwindConfig.theme.extend.colors;

    const expectedColors = Object.fromEntries(
      themeColorTokens.map((token) => [token, `rgb(var(--color-${token}) / <alpha-value>)`]),
    );
    expect(configuredColors).toEqual(expectedColors);
  });
});
