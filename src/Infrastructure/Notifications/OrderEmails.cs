using System.Globalization;
using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Emails;
using ClassManager.Notifications.Email;

namespace ClassManager.Infrastructure.Notifications;

internal static class OrderEmails
{
    private const string AmountFormat = "N2";
    private const string QuantitySeparator = " × ";

    private static readonly CultureInfo SpanishCulture = CultureInfo.GetCultureInfo("es-ES");

    public static EmailLine Line(int quantity, string name, decimal total, string currencyCode) =>
        new(quantity > 1 ? quantity + QuantitySeparator + name : name, Amount(total, currencyCode));

    public static string Amount(decimal amount, string currencyCode) =>
        $"{amount.ToString(AmountFormat, SpanishCulture)} {currencyCode}";

    public static EmailMessage To(OrderEmailRecipient recipient, long businessId, EmailText text) =>
        new(recipient.Email, text.Subject, text.Content, businessId);
}

internal sealed record OrderEmailRecipient(string Email, IEmailTexts Texts);
