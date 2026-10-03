import { lintWithLayoutRules } from "../lintWithLayoutRules";

describe("When component returns a small pill", () => {
  it("Then it is not reported", () => {
    const messages = lintWithLayoutRules(`
      function InactiveChip() {
        return <StatusPill label="Inactivo" tone="neutral" isSmall />;
      }
    `);

    expect(messages).toEqual([]);
  });
});
