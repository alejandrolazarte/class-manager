import { lintWithLayoutRules } from "../lintWithLayoutRules";

describe("When small pill overlays an image", () => {
  it("Then it is not reported", () => {
    const messages = lintWithLayoutRules(`
      const overlay = (
        <View className="absolute left-2.5 top-2.5">
          <StatusPill label="Agotado" tone="neutral" isSmall />
        </View>
      );
    `);

    expect(messages).toEqual([]);
  });
});
