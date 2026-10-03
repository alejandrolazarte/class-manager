import { lintWithLayoutRules } from "../lintWithLayoutRules";

describe("When inactive chip sits beside the text column", () => {
  it("Then it is reported", () => {
    const messages = lintWithLayoutRules(`
      const card = (
        <Card className="flex-row items-center gap-3">
          <AppText className="min-w-0 flex-1">{instructor.fullName}</AppText>
          {instructor.isActive ? null : <InactiveChip />}
        </Card>
      );
    `);

    expect(messages.map((message) => message.ruleId)).toEqual(["layout/pills-in-title"]);
  });
});
