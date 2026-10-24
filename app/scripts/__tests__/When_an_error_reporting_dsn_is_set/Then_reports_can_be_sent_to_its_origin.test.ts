const { contentSecurityPolicy } = jest.requireActual("../../writeStaticWebAppConfig.js");

describe("When an error reporting dsn is set", () => {
  it("Then reports can be sent to its origin", () => {
    const policy: string = contentSecurityPolicy(
      "https://api.example.com",
      undefined,
      "https://public-key@o1.ingest.de.sentry.io/2",
    );

    expect(policy).toContain(
      "connect-src 'self' https://api.example.com https://o1.ingest.de.sentry.io;",
    );
  });
});
