import { lintWithLayoutRules } from "../lintWithLayoutRules";

describe("When small pill is inside the title", () => {
  it("Then it is not reported", () => {
    const messages = lintWithLayoutRules(`
      const title = (
        <TitleWithPills title={member.fullName}>
          {member.isCurrentUser ? <StatusPill label="Vos" tone="primary" isSmall /> : null}
          {member.isActive ? null : <InactiveChip />}
        </TitleWithPills>
      );
    `);

    expect(messages).toEqual([]);
  });
});
