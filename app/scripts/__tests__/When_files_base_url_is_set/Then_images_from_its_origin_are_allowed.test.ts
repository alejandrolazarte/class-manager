const { contentSecurityPolicy } = jest.requireActual("../../writeWebHeaders.js");

describe("When files base url is set", () => {
  it("Then images from its origin are allowed", () => {
    const policy: string = contentSecurityPolicy(
      "https://api.example.com",
      "https://files.example.com/public-files/",
    );

    expect(policy).toContain("img-src 'self' data: blob: https://files.example.com;");
  });
});
