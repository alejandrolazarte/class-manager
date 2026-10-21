const appConfig = jest.requireActual("../../app.config.js");

const baseConfig = {
  name: "Laztty",
  web: { output: "single", favicon: "./assets/images/favicon.png" },
  extra: { router: {}, eas: { projectId: "project" } },
};

describe("When the app target is the backoffice", () => {
  const previousAppTarget = process.env.APP_TARGET;

  afterEach(() => {
    if (previousAppTarget === undefined) {
      delete process.env.APP_TARGET;
    } else {
      process.env.APP_TARGET = previousAppTarget;
    }
  });

  it("Then its routes, name and favicon are used", () => {
    process.env.APP_TARGET = "backoffice";

    const config = appConfig({ config: baseConfig });

    expect(config).toEqual({
      name: "Laztty Admin",
      web: { output: "single", favicon: "./backoffice-web/favicon.png" },
      extra: { router: { root: "./backoffice" }, eas: { projectId: "project" } },
    });
  });
});
