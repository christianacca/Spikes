namespace Eca.Commons.Validation
{
    /// <summary>
    /// Defines the signature of a standard validation method
    /// </summary>
    /// <param name="brokenRules">A list of zero to many broken rules that the method should return</param>
    /// <returns><c>true</c> if there are no rules that are broken (ie <paramref name="brokenRules"/> is an empty list)</returns>
    public delegate bool BrokenRuleMethod(out BrokenRules brokenRules);

    /// <summary>
    /// Defines the signature of a standard validation method
    /// </summary>
    /// <param name="arg">The one parameter that the method is expecting</param>
    /// <param name="brokenRules">A list of zero to many broken rules that the method should return</param>
    /// <returns><c>true</c> if there are no rules that are broken (ie <paramref name="brokenRules"/> is an empty list)</returns>
    public delegate bool BrokenRuleMethod<T>(T arg, out BrokenRules brokenRules);
}