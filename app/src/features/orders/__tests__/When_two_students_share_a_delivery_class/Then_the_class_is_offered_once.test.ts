import { deliveryClassOptions } from "@/features/orders/counterSale";

describe("When two students share a delivery class", () => {
  it("Then the class is offered once", () => {
    const options = deliveryClassOptions([
      { classGroupId: "beginners", classGroupName: "Natación inicial", studentFullName: "Lucía" },
      { classGroupId: "beginners", classGroupName: "Natación inicial", studentFullName: "Tomás" },
      { classGroupId: "advanced", classGroupName: "Natación avanzada", studentFullName: "Tomás" },
    ]);

    expect(options).toEqual([
      { classGroupId: "beginners", hint: "Natación inicial · Lucía, Tomás" },
      { classGroupId: "advanced", hint: "Natación avanzada · Tomás" },
    ]);
  });
});
