namespace ClassManager.Backoffice.Core.Brands;

public static class BrandErrorCodes
{
    public const string BrandNotFound = "backoffice.brand_not_found";
    public const string BrandNotFoundMessage = "There is no brand with that id.";
    public const string NoSubscription = "backoffice.brand_without_subscription";
    public const string NoSubscriptionMessage = "The brand has no subscription to add an add-on to.";
    public const string AddOnNotFound = "backoffice.add_on_not_found";
    public const string AddOnNotFoundMessage = "The brand's current subscription has no such add-on.";
    public const string AddOnReplaced = "backoffice.add_on_replaced";
    public const string AddOnReplacedMessage = "Another add-on for that feature was given after this one was removed.";
    public const string UnknownPlan = "backoffice.unknown_plan";
    public const string UnknownPlanMessage = "There is no active plan with that code.";
    public const string UnknownFeature = "backoffice.unknown_feature";
    public const string UnknownFeatureMessage = "There is no feature with that code.";
    public const string LimitOnUncountedFeature = "backoffice.limit_on_uncounted_feature";
    public const string LimitOnUncountedFeatureMessage = "Only a counted feature can have a limit.";
    public const string NegativeLimit = "backoffice.negative_limit";
    public const string NegativeLimitMessage = "A limit can't be negative.";
    public const string NegativePrice = "backoffice.negative_price";
    public const string NegativePriceMessage = "A price can't be negative.";
    public const string InvalidCurrency = "backoffice.invalid_currency";
    public const string InvalidCurrencyMessage = "The currency must be an ISO 4217 code of three letters.";
    public const string NoteTooLong = "backoffice.note_too_long";
    public const string NoteTooLongMessage = "The note must be at most 200 characters.";
    public const string EndDateInThePast = "backoffice.end_date_in_the_past";
    public const string EndDateInThePastMessage = "The end date can't be before today.";
}
