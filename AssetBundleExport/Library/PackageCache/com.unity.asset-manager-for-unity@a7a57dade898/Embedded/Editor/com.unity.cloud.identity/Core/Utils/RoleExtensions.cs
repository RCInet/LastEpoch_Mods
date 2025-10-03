using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// Helper methods for <see cref="IEnumerable{Role}"/>.
    /// </summary>
    static class RoleExtensions
    {
        /// <summary>
        /// Return if an <see cref="IEnumerable{Role}"/> contains any <see cref="Role"/>.
        /// </summary>
        /// <param name="roles">An <see cref="IEnumerable{Role}"/>.</param>
        /// <param name="role">The <see cref="Role"/> to look for.</param>
        /// <returns>If an <see cref="IEnumerable{Role}"/> contains any <see cref="Role"/>.</returns>
        public static bool HasRole(this IEnumerable<Role> roles, Role role)
        {
            return roles.Any(enumRole => enumRole.Equals(role));
        }
    }
}
