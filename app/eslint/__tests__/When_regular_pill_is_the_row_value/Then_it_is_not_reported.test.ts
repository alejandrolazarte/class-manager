import { lintWithLayoutRules } from "../lintWithLayoutRules";

describe("When regular pill is the row value", () => {
  it("Then it is not reported", () => {
    const messages = lintWithLayoutRules(`
      const row = (
        <Card className="flex-row items-center gap-3">
          <View className="min-w-0 flex-1">
            <AppText>{clientFee.clientFullName}</AppText>
          </View>
          <StatusPill label={statusLabel} tone="success" />
        </Card>
      );
    `);

    expect(messages).toEqual([]);
  });
});
