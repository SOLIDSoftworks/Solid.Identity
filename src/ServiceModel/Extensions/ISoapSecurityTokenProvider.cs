using System.Threading.Tasks;

namespace Solid.Extensions.ServiceModel
{
    /// <summary>
    /// An interface that describes a class that can provide a security token in <see cref="string"/> form.
    /// </summary>
    public interface ISoapSecurityTokenProvider
    {
        /// <summary>
        /// Gets a security token.
        /// </summary>
        /// <returns>An awaitable <see cref="ValueTask{TResult}"/> of type <see cref="string"/>.</returns>
        ValueTask<string> GetSecurityTokenAsync();
    }
}