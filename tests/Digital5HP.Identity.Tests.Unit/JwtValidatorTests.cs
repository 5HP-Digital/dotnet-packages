namespace Digital5HP.Identity.Tests.Unit;

using System.Security.Claims;

using Digital5HP.Identity.Concrete;
using Digital5HP.Test;

using FluentAssertions;

using Microsoft.IdentityModel.Tokens;

using Xunit;

public class JwtValidatorTests : UnitFixtureFor<JwtValidator>
{
    private const string AUDIENCE_AND_ISSUER = "5hp.digital";
    private const string SECURITY_KEY = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

    protected override JwtValidator CreateSut()
    {
        return new JwtValidator();
    }

    [Fact]
    public void ValidateToken_Expired_Failed()
    {
        // Arrange

        // token has expiration in 2020-01-01 12:10AM
        const string TOKEN =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3VzZXJkYXRhIjoiVGVzdCIsImlhdCI6IjE1Nzc4NjU2MDAiLCJuYmYiOjE1Nzc4NjU2MDAsImV4cCI6MTU3Nzg2NjIwMCwiaXNzIjoiNWhwLmRpZ2l0YWwiLCJhdWQiOiI1aHAuZGlnaXRhbCJ9.3GyAnei4sjKXTcEHyYDbU_PJVPuoefv6bTRnPHwsKuo";

        var options = new JwtOptions
        {
            Audience = AUDIENCE_AND_ISSUER,
            Issuer = AUDIENCE_AND_ISSUER,
            SecurityKey = SECURITY_KEY
        };

        // Act
        var exception = Assert.Throws<TokenException>(() => this.Sut.ValidateToken(TOKEN, options));

        // Assert
        exception.Should()
                 .NotBeNull();
        exception.InnerException.Should()
            .NotBeNull();
        exception.InnerException.Should()
            .BeOfType<SecurityTokenExpiredException>();
    }

    [Fact]
    public void ValidateToken_NotYetValid_Failed()
    {
        // Arrange

        // token issued at 2030-12-31 12:00AM
        // token has expiration in 2030-12-31 12:10AM
        const string TOKEN =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3VzZXJkYXRhIjoiVGVzdCIsImlhdCI6IjE5MjQ5MDU2MDAiLCJuYmYiOjE5MjQ5MDU2MDAsImV4cCI6MTkyNDkwNjIwMCwiaXNzIjoiNWhwLmRpZ2l0YWwiLCJhdWQiOiI1aHAuZGlnaXRhbCJ9.DnWImWwsTNky5z7upX2Xjq1VeW0drdroVBozkWqqCWo";


        var options = new JwtOptions
        {
            Audience = AUDIENCE_AND_ISSUER,
            Issuer = AUDIENCE_AND_ISSUER,
            SecurityKey = SECURITY_KEY
        };

        // Act
        var exception = Assert.Throws<TokenException>(() => this.Sut.ValidateToken(TOKEN, options));

        // Assert
        exception.Should()
                 .NotBeNull();
        exception.InnerException.Should()
            .NotBeNull();
        exception.InnerException.Should()
            .BeOfType<SecurityTokenNotYetValidException>();
    }

    [Fact]
    public void ValidateToken_InvalidAudience_Succeed()
    {
        // Arrange

        // token issued at 2020-01-01 12:00AM
        // token has expiration in 2030-12-31 12:00AM
        const string TOKEN =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3VzZXJkYXRhIjoiVGVzdCIsImlhdCI6IjE1Nzc4MzY4MDAiLCJuYmYiOjE1Nzc4MzY4MDAsImV4cCI6MTkyNDkwNTYwMCwiaXNzIjoiNWhwLmRpZ2l0YWwiLCJhdWQiOiI1aHAuZGlnaXRhbCJ9.Nes-OjlHSBd0O1a01p1Hc5NccWm3BqRveyRiBVJArWU";

        var options = new JwtOptions
        {
            Audience = "SomeOtherAudience",
            Issuer = AUDIENCE_AND_ISSUER,
            SecurityKey = SECURITY_KEY
        };

        // Act
        var exception = Assert.Throws<TokenException>(() => this.Sut.ValidateToken(TOKEN, options));

        // Assert
        exception.Should()
                 .NotBeNull();
        exception.InnerException.Should()
            .NotBeNull();
        exception.InnerException.Should()
            .BeOfType<SecurityTokenInvalidAudienceException>();
    }

    [Fact]
    public void ValidateToken_InvalidIssuer_Succeed()
    {
        // Arrange

        // token issued at 2020-01-01 12:00AM
        // token has expiration in 2030-12-31 12:00AM
        const string TOKEN =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3VzZXJkYXRhIjoiVGVzdCIsImlhdCI6IjE1Nzc4MzY4MDAiLCJuYmYiOjE1Nzc4MzY4MDAsImV4cCI6MTkyNDkwNTYwMCwiaXNzIjoiNWhwLmRpZ2l0YWwiLCJhdWQiOiI1aHAuZGlnaXRhbCJ9.Nes-OjlHSBd0O1a01p1Hc5NccWm3BqRveyRiBVJArWU";

        var options = new JwtOptions
        {
            Audience = AUDIENCE_AND_ISSUER,
            Issuer = "SomeOtherAudience",
            SecurityKey = SECURITY_KEY,
        };

        // Act
        var exception = Assert.Throws<TokenException>(() => this.Sut.ValidateToken(TOKEN, options));

        // Assert
        exception.Should()
                 .NotBeNull();
        exception.InnerException.Should()
            .NotBeNull();
        exception.InnerException.Should()
            .BeOfType<SecurityTokenInvalidIssuerException>();
    }

    [Fact]
    public void ValidateToken_InvalidSecurityKey_Succeed()
    {
        // Arrange

        // token issued at 2020-01-01 12:00AM
        // token has expiration in 2030-12-31 12:00AM
        const string TOKEN =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3VzZXJkYXRhIjoiVGVzdCIsImlhdCI6IjE1Nzc4MzY4MDAiLCJuYmYiOjE1Nzc4MzY4MDAsImV4cCI6MTkyNDkwNTYwMCwiaXNzIjoiNWhwLmRpZ2l0YWwiLCJhdWQiOiI1aHAuZGlnaXRhbCJ9.Nes-OjlHSBd0O1a01p1Hc5NccWm3BqRveyRiBVJArWU";

        var options = new JwtOptions
        {
            Audience = AUDIENCE_AND_ISSUER,
            Issuer = AUDIENCE_AND_ISSUER,
            SecurityKey = "1234567890AAAAAAABBBBBBBBBCCC",
        };

        // Act
        var exception = Assert.Throws<TokenException>(() => this.Sut.ValidateToken(TOKEN, options));

        // Assert
        exception.Should()
                 .NotBeNull();
        exception.InnerException.Should()
            .NotBeNull();
        exception.InnerException.Should()
            .BeOfType<SecurityTokenSignatureKeyNotFoundException>();
    }

    [Fact]
    public void ValidateToken_Succeed()
    {
        // Arrange

        // token issued at 2020-01-01 12:00AM
        // token has expiration in 2030-12-31 12:00AM
        const string TOKEN =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3VzZXJkYXRhIjoiVGVzdCIsImlhdCI6IjE1Nzc4MzY4MDAiLCJuYmYiOjE1Nzc4MzY4MDAsImV4cCI6MTkyNDkwNTYwMCwiaXNzIjoiNWhwLmRpZ2l0YWwiLCJhdWQiOiI1aHAuZGlnaXRhbCJ9.Nes-OjlHSBd0O1a01p1Hc5NccWm3BqRveyRiBVJArWU";
        const string EXPECTED_CLAIM_VALUE = "Test";
        const string EXPECTED_CLAIM_TYPE = ClaimTypes.UserData;

        var options = new JwtOptions
        {
            Audience = AUDIENCE_AND_ISSUER,
            Issuer = AUDIENCE_AND_ISSUER,
            SecurityKey = SECURITY_KEY,
        };

        // Act
        var result = this.Sut.ValidateToken(TOKEN, options);

        // Assert
        result.Should()
              .NotBeNull();
        result.Claims.Should()
              .ContainSingle(c => c.Type == EXPECTED_CLAIM_TYPE && c.Value == EXPECTED_CLAIM_VALUE);
    }
}
