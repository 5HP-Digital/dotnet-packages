namespace Digital5HP.Identity.Tests.Unit;

using System;
using System.Security.Claims;

using Digital5HP.Identity.Concrete;
using Digital5HP.Test;

using FluentAssertions;
using FluentAssertions.Extensions;

using Moq;

using Xunit;

public class JwtBuilderTests : UnitFixtureFor<JwtBuilder>
{
    private const string AUDIENCE_AND_ISSUER = "5hp.digital";
    private const string SECURITY_KEY = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
    private const int EXPIRES_MINUTES = 10;

    private readonly Mock<ITimeProvider> timeProviderMock;

    public JwtBuilderTests()
    {
        this.timeProviderMock = this.CreateMock<ITimeProvider>();
    }

    protected override JwtBuilder CreateSut()
    {
        return new JwtBuilder(this.timeProviderMock.Object);
    }

    [Fact]
    public void Build_Succeed()
    {
        // Arrange
        const string EXPECTED_RESULT =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3VzZXJkYXRhIjoiVGVzdCIsImlhdCI6IjE5MjQ5MDU2MDAiLCJuYmYiOjE5MjQ5MDU2MDAsImV4cCI6MTkyNDkwNjIwMCwiaXNzIjoiNWhwLmRpZ2l0YWwiLCJhdWQiOiI1aHAuZGlnaXRhbCJ9.DnWImWwsTNky5z7upX2Xjq1VeW0drdroVBozkWqqCWo";
        var current = new DateTime(2030, 12, 31, 0, 0, 0).AsUtc();
        const string TOKEN_TYPE = "Bearer";

        var options = new JwtOptions
        {
            Audience = AUDIENCE_AND_ISSUER,
            Issuer = AUDIENCE_AND_ISSUER,
            SecurityAlgorithm = SecurityAlgorithm.SHA256,
            SecurityKey = SECURITY_KEY,
            ExpiresMinutes = EXPIRES_MINUTES
        };

        this.timeProviderMock.Setup(provider => provider.Now)
            .Returns(current);

        // Act
        var result = this.Sut.Build(options, new Claim(ClaimTypes.UserData, "Test"));

        // Assert
        result.Should()
              .NotBeNull();

        result.IssuedAt.Should()
              .Be(((DateTimeOffset)current).ToUnixTimeSeconds());
        result.AccessToken
              .Should()
              .Be(EXPECTED_RESULT);
        result.ExpiresIn.Should()
              .Be(
                   (long)TimeSpan.FromMinutes(EXPIRES_MINUTES)
                                  .TotalSeconds);
        result.TokenType.Should()
              .Be(TOKEN_TYPE);

        this.timeProviderMock.Verify(provider => provider.Now, Times.Once);
    }
}
