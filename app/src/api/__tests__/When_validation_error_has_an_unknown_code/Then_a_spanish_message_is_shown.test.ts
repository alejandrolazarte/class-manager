import { getFieldErrors } from "@/api/problemDetails";
import { translate } from "@/i18n/translate";

describe("When validation error has an unknown code", () => {
  it("Then a spanish message is shown", () => {
    const fieldErrors = getFieldErrors({
      status: 400,
      code: "validation",
      errors: { name: ["Name is too long."] },
    });

    expect(fieldErrors).toEqual({ name: translate("apiErrors.invalidValue") });
  });
});
