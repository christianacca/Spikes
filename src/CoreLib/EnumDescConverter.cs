using System;
using System.ComponentModel;
using System.Globalization;

namespace Eca.Commons
{
    /// <summary>
    /// Decoarate an Enum so that the Enum is converted to and from the string assigned by <see cref="DescriptionAttribute"/> 
    /// to each enum symbol.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default an Enum is represented as a string by making the enum symbol a string literal. So for example,
    /// the enum symbol <c>TestEnum.SymbolOne</c> would have a string representation of <c>"SymbolOne"</c>.
    /// </para>
    /// <para>
    /// By decorating the enum like so <c>[TypeConverter(typeof(EnumDescConverter))]</c>, an enum symbol will now be
    /// represeted as the string that's assigned to it using the <see cref="DescriptionAttribute"/>
    /// </para>
    /// <para>
    /// Any enum symbol that has not been assigned a new string using <see cref="DescriptionAttribute"/>, will fall
    /// back to it's default representation.
    /// </para>
    /// </remarks>
    /// <seealso cref="TypeConverter"/>
    public class EnumDescConverter : EnumConverter
    {
        #region Member Variables

        protected Type _enumType;

        #endregion


        #region Constructors

        public EnumDescConverter(Type type)
            : base(type)
        {
            _enumType = type;
        }

        #endregion


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (ReferenceEquals(value, null) || String.Empty.Equals(value)) return null;

            if (!(value is string)) return base.ConvertFrom(context, culture, value);

            return ConvertFromString(value);
        }


        private object ConvertFromString(object value)
        {
            try
            {
                return EnumHelper.ParseDescription((string) value, _enumType);
            }
            catch (ArgumentException e)
            {
                throw new FormatException(string.Format("{0} is not a valid value for {1}", value, _enumType.Name),
                                          e);
            }
        }


        public override object ConvertTo(ITypeDescriptorContext context,
                                         CultureInfo culture,
                                         object value,
                                         Type destinationType)
        {
            if (value is Enum && destinationType == typeof (string))
            {
                return ((Enum) value).ToDescriptionString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}