namespace HorsesForCourses.Core;

public class AppUser
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public EmailAddress Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    public AppUser() { }
    private AppUser(string name, EmailAddress email, string password, string role) { Name = name; Email = email; PasswordHash = password; Role = role; }
    public static AppUser From(string name, string email, string pass, string confirmPass, string role)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.");
        var ValidEmail = EmailAddress.From(email);
        if (pass != confirmPass) throw new ArgumentException("Password is not the same as confirmation password.");
        var hasher = new Pbkdf2PasswordHasher();

        return new AppUser(name, ValidEmail, hasher.Hash(pass), role);
    }
}