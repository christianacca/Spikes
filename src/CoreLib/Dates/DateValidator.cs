using System;
using Eca.Commons.Extensions;
using NValidate.Framework;

namespace Eca.Commons.Dates
{
    public class DateValidator : ReferenceTypeValidatorBase<Date, DateValidator>
    {
        #region Constructors

        public DateValidator(Date parameterValue, string parameterName) : base(parameterValue, parameterName) {}
        public DateValidator(Func<Date> argumentReferernce) : base(argumentReferernce) {}

        #endregion


        public DateValidator IsWorkday()
        {
            return IsWorkday(null);
        }


        public DateValidator IsWorkday(String errorMessage)
        {
            if (!ParameterValue.IsWorkDay)
            {
                ThrowArgumentException(
                    errorMessage.IfNullOrEmpty(String.Format("Expected date to be a workday, received {0}.",
                                                             ParameterValue)));
            }
            return this;
        }
    }



    public static class DateValidatorExtensions
    {
        #region Class Members

        public static DateValidator Date(this The source, Func<Date> argumentReferernce)
        {
            return new DateValidator(argumentReferernce);
        }


        public static DateValidator Date(this The source, Date parameterValue, string parameterName)
        {
            return new DateValidator(parameterValue, parameterName);
        }

        #endregion
    }
}