import { formatPhoneNumberForDisplay } from "@/features/clients/phoneNumberFormatting";

describe("When phone has no mobile prefix", () => {
  it("Then it is formatted with area code", () => {
    expect(formatPhoneNumberForDisplay("+541122334455")).toBe("+54 11 2233-4455");
  });
});
