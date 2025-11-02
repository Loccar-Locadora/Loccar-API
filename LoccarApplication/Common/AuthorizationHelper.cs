using LoccarDomain.LoggedUser.Models;

namespace LoccarApplication.Common
{
    public static class AuthorizationHelper
    {
        /// <summary>
        /// Verifica se o usuário tem pelo menos uma das roles de administrador (ADMIN ou EMPLOYEE)
        /// </summary>
        /// <param name="loggedUser">Usuário logado</param>
        /// <returns>True se o usuário tem role de ADMIN ou EMPLOYEE</returns>
        public static bool HasAdminOrEmployeeRole(LoggedUser loggedUser)
        {
            if (loggedUser?.Roles == null || !loggedUser.Roles.Any())
                return false;

            return loggedUser.Roles.Any(role => 
                string.Equals(role, "CLIENT_ADMIN", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(role, "CLIENT_EMPLOYEE", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Verifica se o usuário tem uma role específica
        /// </summary>
        /// <param name="loggedUser">Usuário logado</param>
        /// <param name="requiredRole">Role necessária</param>
        /// <returns>True se o usuário tem a role especificada</returns>
        public static bool HasRole(LoggedUser loggedUser, string requiredRole)
        {
            if (loggedUser?.Roles == null || !loggedUser.Roles.Any() || string.IsNullOrEmpty(requiredRole))
                return false;

            return loggedUser.Roles.Any(role => 
                string.Equals(role, requiredRole, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Verifica se o usuário tem pelo menos uma das roles especificadas
        /// </summary>
        /// <param name="loggedUser">Usuário logado</param>
        /// <param name="requiredRoles">Roles necessárias</param>
        /// <returns>True se o usuário tem pelo menos uma das roles especificadas</returns>
        public static bool HasAnyRole(LoggedUser loggedUser, params string[] requiredRoles)
        {
            if (loggedUser?.Roles == null || !loggedUser.Roles.Any() || requiredRoles == null || !requiredRoles.Any())
                return false;

            return loggedUser.Roles.Any(userRole =>
                requiredRoles.Any(requiredRole =>
                    string.Equals(userRole, requiredRole, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Verifica se o usuário está autenticado e tem pelo menos uma role
        /// </summary>
        /// <param name="loggedUser">Usuário logado</param>
        /// <returns>True se o usuário está autenticado</returns>
        public static bool IsAuthenticated(LoggedUser loggedUser)
        {
            return loggedUser != null && 
                   loggedUser.Authenticated && 
                   loggedUser.Roles != null && 
                   loggedUser.Roles.Any();
        }

        /// <summary>
        /// Verifica se o usuário é um administrador (ADMIN)
        /// </summary>
        /// <param name="loggedUser">Usuário logado</param>
        /// <returns>True se o usuário tem role ADMIN</returns>
        public static bool IsAdmin(LoggedUser loggedUser)
        {
            return HasRole(loggedUser, "ADMIN");
        }

        /// <summary>
        /// Verifica se o usuário é um funcionário (EMPLOYEE)
        /// </summary>
        /// <param name="loggedUser">Usuário logado</param>
        /// <returns>True se o usuário tem role EMPLOYEE</returns>
        public static bool IsEmployee(LoggedUser loggedUser)
        {
            return HasRole(loggedUser, "EMPLOYEE");
        }
    }
}
