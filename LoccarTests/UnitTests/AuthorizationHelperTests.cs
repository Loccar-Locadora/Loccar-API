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
        [InlineData("CLIENT_ADMIN", true)]
        [InlineData("client_admin", true)]
        [InlineData("Client_Admin", true)]
        [InlineData("CLIENT_EMPLOYEE", true)]
        [InlineData("client_employee", true)]
        [InlineData("Client_Employee", true)]
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
                Roles = new List<string> { "CLIENT_USER", "CLIENT_ADMIN", "GUEST" },
                Authenticated = true
            };

            // Act
            var result = AuthorizationHelper.HasAdminOrEmployeeRole(loggedUser);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("CLIENT_ADMIN", "CLIENT_ADMIN", true)]
        [InlineData("CLIENT_EMPLOYEE", "CLIENT_EMPLOYEE", true)]
        [InlineData("CLIENT_USER", "CLIENT_USER", true)]
        [InlineData("ADMIN", "ADMIN", true)]
        [InlineData("EMPLOYEE", "EMPLOYEE", true)]
        [InlineData("COMMON_USER", "COMMON_USER", true)]
        [InlineData("CLIENT_ADMIN", "ADMIN", false)]
        [InlineData("CLIENT_ADMIN", "CLIENT_EMPLOYEE", false)]
        [InlineData("CLIENT_ADMIN", "CLIENT_USER", false)]
        [InlineData("CLIENT_EMPLOYEE", "CLIENT_ADMIN", false)]
        [InlineData("CLIENT_USER", "CLIENT_ADMIN", false)]
        [InlineData("", "CLIENT_ADMIN", false)]
        [InlineData(null, "CLIENT_ADMIN", false)]
        public void HasRoleReturnsCorrectResult(string userRole, string requiredRole, bool expected)
        {
            // Arrange
            var loggedUser = new LoccarDomain.LoggedUser.Models.LoggedUser
            {
                Roles = userRole != null ? new List<string> { userRole } : null
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
                Roles = new List<string> { "CLIENT_EMPLOYEE" },
                Authenticated = true
            };

            // Act
            var result = AuthorizationHelper.HasAnyRole(loggedUser, "CLIENT_ADMIN", "CLIENT_EMPLOYEE", "MANAGER");

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
            var result = AuthorizationHelper.HasAnyRole(loggedUser, "CLIENT_ADMIN", "CLIENT_EMPLOYEE", "MANAGER");

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
        [InlineData("CLIENT_ADMIN", true)]
        [InlineData("client_admin", true)]
        [InlineData("Client_Admin", true)]
        [InlineData("CLIENT_EMPLOYEE", false)]
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
        [InlineData("CLIENT_EMPLOYEE", true)]
        [InlineData("client_employee", true)]
        [InlineData("CLIENT_ADMIN", false)]
        [InlineData("CLIENT_USER", false)]
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
