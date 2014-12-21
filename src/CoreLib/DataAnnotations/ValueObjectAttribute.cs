using System;

namespace Eca.Commons.DataAnnotations
{
    /// <summary>
    /// Marks a class as being a Value Object (as opposed to being an another type such as Entity)
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ValueObjectAttribute : Attribute {}
}