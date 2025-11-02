using System.Collections.Generic;
using FluentAssertions;
using LoccarApplication.Common;
using LoccarDomain.LoggedUser.Models;
using Xunit;

namespace LoccarTests.UnitTests
{
    public class AuthorizationHelperTests
    {
        [Theory]
        [InlineData("ADMIN", true)]
        [InlineData("admin", true)]
        [InlineData("Admin", true)]
        [InlineData("EMPLOYEE", true)]
        [InlineData("employee", true)]
        [InlineData("Employee", true)]
        [InlineData("USER", false)]
        [InlineData("GUEST", false)]
        [InlineData("", false)]
        public void HasAdminOrEmployeeRoleReturnsCorrectResult(string role, bool expected)
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { role },
                Authenticated = true
            };

            // Act
            var result = AuthorizationHelper.HasAdminOrEmployeeRole(loggedUser);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void HasAdminOrEmployeeRoleWhenNullUserReturnsFalse()
        {
            // Act
            var result = AuthorizationHelper.HasAdminOrEmployeeRole(null);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void HasAdminOrEmployeeRoleWhenNullRolesReturnsFalse()
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = null,
                Authenticated = true
            };

            // Act
            var result = AuthorizationHelper.HasAdminOrEmployeeRole(loggedUser);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void HasAdminOrEmployeeRoleWhenEmptyRolesReturnsFalse()
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string>(),
                Authenticated = true
            };

            // Act
            var result = AuthorizationHelper.HasAdminOrEmployeeRole(loggedUser);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void HasAdminOrEmployeeRoleWithMultipleRolesIncludingAdminReturnsTrue()
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { "USER", "ADMIN", "GUEST" },
                Authenticated = true
            };

            // Act
            var result = AuthorizationHelper.HasAdminOrEmployeeRole(loggedUser);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("ADMIN", "ADMIN", true)]
        [InlineData("admin", "ADMIN", true)]
        [InlineData("Admin", "admin", true)]
        [InlineData("USER", "ADMIN", false)]
        [InlineData("", "ADMIN", false)]
        public void HasRoleReturnsCorrectResult(string userRole, string requiredRole, bool expected)
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { userRole },
                Authenticated = true
            };

            // Act
            var result = AuthorizationHelper.HasRole(loggedUser, requiredRole);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void HasAnyRoleWithMultipleRequiredRolesReturnsTrue()
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { "EMPLOYEE" },
                Authenticated = true
            };

            // Act
            var result = AuthorizationHelper.HasAnyRole(loggedUser, "ADMIN", "EMPLOYEE", "MANAGER");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasAnyRoleWithNoMatchingRolesReturnsFalse()
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { "USER" },
                Authenticated = true
            };

            // Act
            var result = AuthorizationHelper.HasAnyRole(loggedUser, "ADMIN", "EMPLOYEE", "MANAGER");

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(false, true, false)]
        [InlineData(true, false, false)]
        [InlineData(false, false, false)]
        public void IsAuthenticatedReturnsCorrectResult(bool authenticated, bool hasRoles, bool expected)
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Authenticated = authenticated,
                Roles = hasRoles ? new List<string> { "USER" } : new List<string>()
            };

            // Act
            var result = AuthorizationHelper.IsAuthenticated(loggedUser);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void IsAuthenticatedWhenNullUserReturnsFalse()
        {
            // Act
            var result = AuthorizationHelper.IsAuthenticated(null);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("ADMIN", true)]
        [InlineData("admin", true)]
        [InlineData("EMPLOYEE", false)]
        [InlineData("USER", false)]
        public void IsAdminReturnsCorrectResult(string role, bool expected)
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { role },
                Authenticated = true
            };

            // Act
            var result = AuthorizationHelper.IsAdmin(loggedUser);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("EMPLOYEE", true)]
        [InlineData("employee", true)]
        [InlineData("ADMIN", false)]
        [InlineData("USER", false)]
        public void IsEmployeeReturnsCorrectResult(string role, bool expected)
        {
            // Arrange
            var loggedUser = new LoggedUser
            {
                Roles = new List<string> { role },
                Authenticated = true
            };

            // Act
            var result = AuthorizationHelper.IsEmployee(loggedUser);

            // Assert
            result.Should().Be(expected);
        }
    }
}
