using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// Helper methods for <see cref="IEnumerable{Permission}"/>.
    /// </summary>
    static class PermissionExtensions
    {
        /// <summary>
        /// Return if an <see cref="IEnumerable{Permission}"/> contains any <see cref="Permission"/>.
        /// </summary>
        /// <param name="permissions">An <see cref="IEnumerable{Permission}"/>.</param>
        /// <param name="permission">The <see cref="Permission"/> to look for.</param>
        /// <returns>If an <see cref="IEnumerable{Permission}"/> contains any <see cref="Permission"/>.</returns>
        public static bool HasPermission(this IEnumerable<Permission> permissions, Permission permission)
        {
            return permissions.Any(enumPermission => enumPermission.Equals(permission));
        }
    }
}
