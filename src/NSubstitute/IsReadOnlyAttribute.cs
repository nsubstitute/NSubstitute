namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Attribute used by the compiler to mark the readonly by-ref arguments (`in` parameters).
    /// The reason this attribute is defined explicitly is that we need to access it's type it in our code.
    /// </summary>
    internal class IsReadOnlyAttribute: Attribute
    {
    }
}