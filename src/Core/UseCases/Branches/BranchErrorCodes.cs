namespace ClassManager.Core.UseCases.Branches;

public static class BranchErrorCodes
{
    public const string Unavailable = "branch.unavailable";
    public const string LastBrandOwner = "brand_owner.last";

    public const string UnavailableMessage = "You don't have access to that branch, or your session expired.";
    public const string LastBrandOwnerMessage = "The brand needs at least one brand owner.";
}
