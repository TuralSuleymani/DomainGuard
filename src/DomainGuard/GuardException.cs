namespace DomainGuard
{
    /// <summary>
    /// Exception thrown when a guard clause validation fails.
    /// </summary>
    public sealed class GuardException : Exception
    {
        public GuardException(string message) : base(message) { }
    }
}
