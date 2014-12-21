using System;

namespace Eca.Commons
{
    /// <summary>
    /// Used to mark types as being like a type built-in to the .Net framework  (like int, string, datetime, etc)
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IsBuiltinTypeAttribute : Attribute {}
}