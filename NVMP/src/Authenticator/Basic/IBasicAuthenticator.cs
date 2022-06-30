namespace NVMP.Authenticator.Basic
{
    /// <summary>
    /// A primitive authenticator that checks a text file for known banned IP addresses, and authenticates any player otherwise.
    /// </summary>
    public interface IBasicAuthenticator : IAuthenticator
    {
    }
}
