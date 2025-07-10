public record EmailAddress
{
    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    public static EmailAddress From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.");
        if (!value.Contains("@"))
            throw new ArgumentException("Invalid email format.");
        return new EmailAddress(value);
    }
}