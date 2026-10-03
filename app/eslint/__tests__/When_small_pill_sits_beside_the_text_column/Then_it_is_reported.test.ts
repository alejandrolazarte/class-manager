import { lintWithLayoutRules } from "../lintWithLayoutRules";

describe("When small pill sits beside the text column", () => {
  it("Then it is reported", () => {
    const messages = lintWithLayoutRules(`
      const card = (
        <Card className="flex-row items-center gap-3">
          <View className="min-w-0 flex-1">
            <AppText>{member.fullName}</AppText>
          </View>
          {member.isCurrentUser ? <StatusPill label="Vos" tone="primary" isSmall /> : null}
        </Card>
      );
    `);

    expect(messages.map((message) => message.ruleId)).toEqual(["layout/pills-in-title"]);
  });
});
