import { getFieldErrors } from "@/api/problemDetails";
import { translate } from "@/i18n/translate";

describe("When validation error is about the password", () => {
  it("Then the password rule is shown", () => {
    const fieldErrors = getFieldErrors({
      status: 400,
      code: "validation",
      errors: { Password: ["Password must be between 10 and 128 characters."] },
    });

    expect(fieldErrors).toEqual({ password: translate("apiErrors.field.password") });
  });
});
