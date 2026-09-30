import { findBrandColorClash } from "@/theme/brandColorClash";

describe("When brand color is close to a status", () => {
  it("Then the clash is reported", () => {
    expect(findBrandColorClash("#d32f2f")).toBe("danger");
    expect(findBrandColorClash("#0076b4")).toBeNull();
  });
});
