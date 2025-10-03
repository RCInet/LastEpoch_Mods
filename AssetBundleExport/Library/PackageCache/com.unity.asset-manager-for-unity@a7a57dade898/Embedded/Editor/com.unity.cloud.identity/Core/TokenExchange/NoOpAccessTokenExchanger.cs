using System.Threading.Tasks;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// An <see cref="IAccessTokenExchanger{T, T}"/> where the T1 input is a string and T2 output is a <see cref="UnityServicesToken"/> and where no exchange operation is executed.
    /// </summary>
    internal class NoOpAccessTokenExchanger: IAccessTokenExchanger<string, UnityServicesToken>
    {
        /// <inheritdoc/>
        public Task<UnityServicesToken> ExchangeAsync(string exchangeToken)
        {
            return Task.FromResult(new UnityServicesToken{ AccessToken = exchangeToken});
        }
    }
}
