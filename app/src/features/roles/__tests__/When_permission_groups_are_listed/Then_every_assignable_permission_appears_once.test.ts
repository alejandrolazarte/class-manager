import { brandOnlyPermissions, everyPermission } from "@/features/members/permissions";
import { permissionGroups, permissionsOf } from "@/features/roles/permissionGroups";

describe("When permission groups are listed", () => {
  it("Then every assignable permission appears once", () => {
    const listed = permissionGroups.flatMap((group) => group.items.flatMap(permissionsOf));

    expect([...listed].sort()).toEqual(
      everyPermission.filter((permission) => !brandOnlyPermissions.includes(permission)).sort(),
    );
  });
});
