using HorsesForCourses.Core;
namespace HorsesForCourses.Tests;

public class EmailAddressTests
{
    [Fact]
    public void Equality()
    {
        var emailOne = EmailAddress.From("someone@somewhere.com");
        var emailTwo = EmailAddress.From("someone@somewhere.com");
        Assert.Equal(emailOne, emailTwo);
        Assert.Equal("someone@somewhere.com", emailOne.Value);
    }

    [Fact]
    public void Cant_Be_Empty()
    {
        var exception = Assert.Throws<ArgumentException>(() => EmailAddress.From(""));
        Assert.Equal("Email cannot be empty.", exception.Message);
    }

    [Fact]
    public void Must_Contain_At()
    {
        var exception = Assert.Throws<ArgumentException>(() => EmailAddress.From("someonesomewhere.com"));
        Assert.Equal("Invalid email format.", exception.Message);
    }
}