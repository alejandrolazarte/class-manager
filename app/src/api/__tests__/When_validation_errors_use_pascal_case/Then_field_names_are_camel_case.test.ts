import { getFieldErrors } from "@/api/problemDetails";

describe("When validation errors use pascal case", () => {
  it("Then field names are camel case", () => {
    const fieldErrors = getFieldErrors({
      status: 400,
      errors: { FullName: ["Too short"], phoneNumber: ["Invalid", "Required"] },
    });

    expect(Object.keys(fieldErrors)).toEqual(["fullName", "phoneNumber"]);
  });
});
