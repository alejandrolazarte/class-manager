const { contentSecurityPolicy } = jest.requireActual("../../writeWebHeaders.js");

describe("When api base url has a path", () => {
  it("Then only the api origin is allowed", () => {
    const policy: string = contentSecurityPolicy("https://api.example.com/v1/");

    expect(policy).toContain("connect-src 'self' https://api.example.com;");
  });
});
