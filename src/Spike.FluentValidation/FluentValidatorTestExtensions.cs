using System;
using System.Linq;
using System.Linq.Expressions;
using Eca.Commons.Reflection;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;

namespace Spike.FluentValidation
{
    public static class FluentValidatorTestExtensions
    {
        #region Class Members

        public static void ShouldHaveValidationErrorFor<T>(this AbstractValidator<T> validator,
                                                           Expression<Func<T, object>> propertyToValidate,
                                                           T objectToBeValidated)
        {
            string propertyName = ReflectionUtil.GetPropertyNames(propertyToValidate);
            ShouldHaveValidationErrorFor(validator, propertyName, objectToBeValidated);
        }


        public static void ShouldHaveValidationErrorFor<T>(this AbstractValidator<T> validator,
                                                           string propertyName,
                                                           T objectToBeValidated)
        {
            ValidationResult result = validator.Validate(objectToBeValidated);
            bool found = result.Errors.Any(failure => failure.PropertyName == propertyName);
            if (!found)
                throw new ValidationTestException(string.Format("Expected to find an error for property {0}",
                                                                propertyName));
        }


        public static void ShouldNotHaveValidationErrorFor<T>(this AbstractValidator<T> validator,
                                                              Expression<Func<T, object>> propertyToValidate,
                                                              T objectToBeValidated)
        {
            string propertyName = ReflectionUtil.GetPropertyNames(propertyToValidate);
            ShouldNotHaveValidationErrorFor(validator, propertyName, objectToBeValidated);
        }


        public static void ShouldNotHaveValidationErrorFor<T>(this AbstractValidator<T> validator,
                                                              string propertyName,
                                                              T objectToBeValidated)
        {
            ValidationResult result = validator.Validate(objectToBeValidated);
            bool found = result.Errors.Any(failure => failure.PropertyName == propertyName);
            if (found)
                throw new ValidationTestException(string.Format("Expected not to find an error for property {0}",
                                                                propertyName));
        }

        #endregion
    }
}