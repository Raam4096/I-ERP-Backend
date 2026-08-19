namespace iERP.Modules.CRM.Application.Common.Validation;

public static class UrlValidation
{
    public static bool BeValidHttpUrlOrEmpty(string? website)
    {
        if (string.IsNullOrWhiteSpace(website))
        {
            return true;
        }

        return Uri.TryCreate(website, UriKind.Absolute, out var uri) &&
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
