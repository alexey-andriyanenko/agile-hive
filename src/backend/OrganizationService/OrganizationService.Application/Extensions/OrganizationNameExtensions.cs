namespace OrganizationService.Application.Extensions;

public static class OrganizationNameExtensions
{
    public static string ToSlug(this string organizationName)
    {
        if (string.IsNullOrWhiteSpace(organizationName))
            return string.Empty;

        // Convert to lower case, trim, and replace spaces with hyphens
        var slug = organizationName
            .Trim()
            .ToLowerInvariant()
            .Replace(" ", "-");

        // Remove invalid characters (keep letters, numbers, and hyphens)
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "");

        // Replace multiple hyphens with a single hyphen
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\-{2,}", "-");

        return slug;
    }
}