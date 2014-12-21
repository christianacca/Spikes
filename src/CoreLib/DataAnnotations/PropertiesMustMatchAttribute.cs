using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Eca.Commons.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class PropertiesMustMatchAttribute : ValidationAttribute
    {
        #region Member Variables

        private const string _defaultErrorMessage = "'{0}' and '{1}' do not match.";

        private readonly object _typeId = new object();

        #endregion


        #region Constructors

        public PropertiesMustMatchAttribute(string originalProperty, string confirmProperty)
            : base(_defaultErrorMessage)
        {
            OriginalProperty = originalProperty;
            ConfirmProperty = confirmProperty;
        }

        #endregion


        #region Properties

        public string ConfirmProperty { get; private set; }

        public string OriginalProperty { get; private set; }

        public override object TypeId
        {
            get { return _typeId; }
        }

        #endregion


        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture,
                                 ErrorMessageString,
                                 OriginalProperty,
                                 ConfirmProperty);
        }


        public override bool IsValid(object value)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
            object originalValue = properties.Find(OriginalProperty, true /* ignoreCase */).GetValue(value);
            object confirmValue = properties.Find(ConfirmProperty, true /* ignoreCase */).GetValue(value);
            return Equals(originalValue, confirmValue);
        }
    }
}