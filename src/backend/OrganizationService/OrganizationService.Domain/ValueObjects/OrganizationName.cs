namespace OrganizationService.Domain.ValueObjects;

using System;
using System.Text.RegularExpressions;

public sealed class OrganizationName
{
    private static readonly Regex AllowedCharsRegex = new(@"^[a-zA-Z0-9\s\-_.]+$", RegexOptions.Compiled);
    private static readonly Regex MultipleSpacesRegex = new(@"\s{2,}", RegexOptions.Compiled);

    public string Value { get; }

    public OrganizationName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Organization name cannot be empty.", nameof(name));
        
        name = name.Trim();
        
        if (MultipleSpacesRegex.IsMatch(name))
            throw new ArgumentException("Organization name cannot contain more than 1 consecutive spaces.", nameof(name));
        
        if (name.Length < 2 || name.Length > 16)
            throw new ArgumentException("Organization name must be between 2 and 16 characters long.", nameof(name));
        
        if (!AllowedCharsRegex.IsMatch(name))
            throw new ArgumentException("Organization name contains forbidden symbols.", nameof(name));

        Value = name;
    }

    public override string ToString() => Value;
    
    public override bool Equals(object? obj) =>
        obj is OrganizationName other && Value.Equals(other.Value, StringComparison.Ordinal);

    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);
}