using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Eca.Commons.Extensions;
using Eca.Commons.Reflection;

namespace Eca.Commons.DataAnnotations
{
    /// <summary>
    /// Declares that out of a set of properties from the decorated class, that at least one of them must have a value supplied
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class AtLeastOnePropertyRequiredAttribute : ValidationAttribute
    {
        #region Member Variables

        private const string _defaultErrorMessage = "A value is required for one of the following: {0}";

        private readonly object _typeId = new object();

        #endregion


        #region Constructors

        public AtLeastOnePropertyRequiredAttribute(params string[] properties)
            : base(_defaultErrorMessage)
        {
            Properties = properties.SafeToList();
        }

        #endregion


        #region Properties

        public ICollection<string> Properties { get; private set; }


        public override object TypeId
        {
            get { return _typeId; }
        }

        #endregion


        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentUICulture,
                                 ErrorMessageString,
                                 Properties.Select(p => String.Format("'{0}'", p)).Join(", "));
        }


        public override bool IsValid(object value)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
            var propertyValues = Properties.Select(name => properties.Find(name, true).GetValue(value)).SkipNulls();

            var hasAtLeastOneValue = propertyValues.Any(v => !Equals(v.GetType().GetDefault(), v));
            return hasAtLeastOneValue;
        }
    }
}