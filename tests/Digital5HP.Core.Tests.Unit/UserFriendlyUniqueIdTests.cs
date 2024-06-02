namespace Digital5HP.Core.Tests.Unit
{
    using System;
    using System.Linq;

    using FluentAssertions;

    using Digital5HP.Test;

    using Xunit;

    [Trait("Category", "Unit")]
    public class UserFriendlyUniqueIdTests : FixtureBase
    {
        [Fact]
        public void ToString_Succeed()
        {
            // Arrange
            var sut = UserFriendlyUniqueId.New();

            // Act
            var result = sut.ToString();

            // Assert
            result.Should()
                  .NotBeNullOrWhiteSpace()
                  .And.Match(s => s.All(ch => UserFriendlyUniqueId.LETTERS.Contains(ch, StringComparison.Ordinal)));
        }

        [Theory]
        [AutoMoqData]
        public void ToString_WithNumOfChars_Succeed(byte num)
        {
            // Arrange
            var sut = UserFriendlyUniqueId.New(num);

            // Act
            var result = sut.ToString();

            // Assert
            result.Should()
                  .NotBeNullOrWhiteSpace()
                  .And.HaveLength(num)
                  .And.Match(s => s.All(ch => UserFriendlyUniqueId.LETTERS.Contains(ch, StringComparison.Ordinal)));
        }

        [Fact]
        public void New_UniqueResults_Succeed()
        {
            // Arrange

            // Act
            var sut1 = UserFriendlyUniqueId.New();
            var sut2 = UserFriendlyUniqueId.New();
            var sut3 = UserFriendlyUniqueId.New();

            // Assert
            sut1.Should()
                .NotBe(sut2)
                .And.NotBe(sut3);
            sut2.Should()
                .NotBe(sut3);
        }

        [Theory]
        [AutoMoqData]
        public void NewWithNum_UniqueResults_Succeed(int num)
        {
            // Arrange

            // Act
            var sut1 = UserFriendlyUniqueId.New(num);
            var sut2 = UserFriendlyUniqueId.New(num);
            var sut3 = UserFriendlyUniqueId.New(num);

            // Assert
            sut1.Should()
                .NotBe(sut2)
                .And.NotBe(sut3);
            sut2.Should()
                .NotBe(sut3);

        }

        [Fact]
        public void NewWithNum_NumIsZero_ThrowException()
        {
            // Arrange

            // Act
            var exception= Record.Exception(() => UserFriendlyUniqueId.New(0));

            // Assert
            exception.Should()
                     .NotBeNull()
                     .And.BeOfType<ArgumentException>();

        }

        [Theory]
        [AutoMoqData]
        public void ConsistentResultsWithGuid_Succeed(Guid guid)
        {
            // Arrange

            // Act
            var sut1 = new UserFriendlyUniqueId(guid);
            var sut2 = new UserFriendlyUniqueId(guid);
            var sut3 = new UserFriendlyUniqueId(guid);

            // Assert
            sut1.Should()
                .Be(sut2)
                .And.Be(sut3);
        }

        [Theory]
        [AutoMoqData]
        public void ConsistentResultsWithByteArray_Succeed(byte[] bytes)
        {
            // Arrange

            // Act
            var sut1 = new UserFriendlyUniqueId(bytes);
            var sut2 = new UserFriendlyUniqueId(bytes);
            var sut3 = new UserFriendlyUniqueId(bytes);

            // Assert
            sut1.Should()
                .Be(sut2)
                .And.Be(sut3);
        }

        [Fact]
        public void CtorWithBytes_IsNull_ThrowsException()
        {
            // Arrange

            // Act
            var exception = Record.Exception(() => new UserFriendlyUniqueId(null));

            // Assert
            exception.Should()
                     .NotBeNull()
                     .And.BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void CtorWithBytes_IsEmpty_ThrowsException()
        {
            // Arrange

            // Act
            var exception = Record.Exception(() => new UserFriendlyUniqueId(Array.Empty<byte>()));

            // Assert
            exception.Should()
                     .NotBeNull()
                     .And.BeOfType<ArgumentException>();
        }

        [Fact]
        public void Equals_ToNull_Succeed()
        {
            // Arrange

            // Act
            var result = new UserFriendlyUniqueId().Equals(null);

            // Assert
            result.Should()
                  .BeFalse();
        }

        [Theory]
        [AutoMoqData]
        public void Equals_ToGuid_Succeed(Guid guid)
        {
            // Arrange

            // Act
            // ReSharper disable once SuspiciousTypeConversion.Global
            var result = new UserFriendlyUniqueId(guid).Equals(guid);

            // Assert
            result.Should()
                  .BeTrue();
        }

        [Fact]
        public void Equals_ToByteArray_Succeed()
        {
            // Arrange
            var bytes = this.CreateMany<byte>(16).ToArray();


            // Act
            // ReSharper disable once SuspiciousTypeConversion.Global
            var result = new UserFriendlyUniqueId(bytes).Equals(bytes);

            // Assert
            result.Should()
                  .BeTrue();
        }

        [Fact]
        public void Equals_ToString_Succeed()
        {
            // Arrange
            var sut1 = UserFriendlyUniqueId.New(20);

            // Act
            // ReSharper disable once SuspiciousTypeConversion.Global
            var result = sut1.Equals(sut1.ToString());

            // Assert
            result.Should()
                  .BeTrue();
        }

        [Fact]
        public void Equals_ToUnknownObject_Succeed()
        {
            // Arrange
            var obj = new object();


            // Act
            // ReSharper disable once SuspiciousTypeConversion.Global
            var sut = new UserFriendlyUniqueId().Equals(obj);

            // Assert
            sut.Should()
               .BeFalse();
        }

        [Theory]
        [AutoMoqData]
        public void GetHashCode_Succeed(Guid guid)
        {
            // Arrange
            var sut1 = new UserFriendlyUniqueId(guid);
            var sut2 = sut1.ToString();

            // Act
            var hash1 = sut1.GetHashCode();
            var hash2 = sut2.GetHashCode(StringComparison.Ordinal);

            // Assert
            hash1.Should()
                 .Be(hash2);
        }
    }
}
