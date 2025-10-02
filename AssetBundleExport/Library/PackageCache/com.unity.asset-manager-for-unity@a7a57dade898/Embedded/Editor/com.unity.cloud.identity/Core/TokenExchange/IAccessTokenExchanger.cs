using System;
using System.Threading.Tasks;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// A class holding the Unity Services access token.
    /// </summary>
    class UnityServicesToken
    {
        /// <summary>
        /// The Unity Services access token.
        /// </summary>
        public string AccessToken { get; set; }
    }

    /// <summary>
    /// An interface to exchange a T1 input value for a T2 output value.
    /// </summary>
    /// <typeparam name="T1">An input token type.</typeparam>
    /// <typeparam name="T2">An output token type.</typeparam>
    interface IAccessTokenExchanger<T1, T2>
    {
        /// <summary>
        /// Returns a T2 exchanged token
        /// </summary>
        /// <param name="exchangeToken">The token input value of type T1.</param>
        /// <returns>
        /// A task that once completed returns a T2 exchanged token from a T1 input value.
        /// </returns>
        Task<T2> ExchangeAsync(T1 exchangeToken);
    }
}
