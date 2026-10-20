const appConfig = jest.requireActual("../../app.config.js");

const baseConfig = { name: "Laztty", extra: { router: {}, eas: { projectId: "project" } } };

describe("When no app target is set", () => {
  const previousAppTarget = process.env.APP_TARGET;

  afterEach(() => {
    if (previousAppTarget === undefined) {
      delete process.env.APP_TARGET;
    } else {
      process.env.APP_TARGET = previousAppTarget;
    }
  });

  it("Then the config is unchanged", () => {
    delete process.env.APP_TARGET;

    expect(appConfig({ config: baseConfig })).toBe(baseConfig);
  });
});
