using System;
using System.Collections.Generic;
using Unity.Cloud.AppLinkingEmbedded;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// Creates the <see cref="CompositeAuthenticatorSettings"/> required to inject in a <see cref="CompositeAuthenticator"/>.
    /// </summary>
    readonly struct CompositeAuthenticatorSettings
    {
        /// <summary>
        /// The prioritized list of <see cref="IAuthenticator"/> to inject in a <see cref="CompositeAuthenticator"/>.
        /// </summary>
        internal readonly IReadOnlyList<IAuthenticator> Authenticators;

        /// <summary>
        /// Creates a <see cref="CompositeAuthenticatorSettings"/> to inject in a <see cref="CompositeAuthenticator"/>.
        /// </summary>
        /// <param name="authenticators">A prioritized list of <see cref="IAuthenticator"/>.</param>
        internal CompositeAuthenticatorSettings(List<IAuthenticator> authenticators)
        {
            Authenticators = authenticators;
        }
    }
}
