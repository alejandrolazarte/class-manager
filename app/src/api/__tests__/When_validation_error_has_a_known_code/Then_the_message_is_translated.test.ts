import { getFieldErrors } from "@/api/problemDetails";
import { translate } from "@/i18n/translate";

describe("When validation error has a known code", () => {
  it("Then the message is translated", () => {
    const fieldErrors = getFieldErrors({
      status: 400,
      code: "class_group.capacity_below_enrolled",
      errors: { capacity: ["Capacity can't be lower than the students currently enrolled."] },
    });

    expect(fieldErrors).toEqual({
      capacity: translate("apiErrors.class_group.capacity_below_enrolled"),
    });
  });
});
