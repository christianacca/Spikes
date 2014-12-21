using System;

namespace Eca.Commons.DataAnnotations
{
    /// <summary>
    /// Determines the order in which a adorned property is rendered on screen
    /// </summary>
    [SkipFormatting]
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class OrderAttribute : Attribute
    {
        private readonly int _order;


        public OrderAttribute(int order)
        {
            _order = order;
        }


        public int Order
        {
            get { return _order; }
        }
    }
}