namespace Eca.Commons.Validation
{
    /// <summary>
    /// Used to define acceptable thresholds for validation performance.
    /// </summary>
    /// <remarks>
    /// Implementors of <see cref="IValidationRunner"/> should use these thresholds to decide when for example to output warnings
    /// when the validation exceeds acceptable thresholds set
    /// </remarks>
    public class ValidationPerformanceThresholds
    {
        #region Properties

        /// <summary>
        /// The duration of a call to <see cref="IValidationProvider{T}.Validate(T,Eca.Commons.Validation.ValidationCallContext)"/>
        /// that is acceptable before a warning should be output. A value of 0 indicates that no warning should ever be output.
        /// </summary>
        public int IndividualValidatorInMilliseconds { get; set; }

        /// <summary>
        /// The total duration of any one call to <see cref="IValidationRunner.Validate{T}(T,ValidationCallContext)"/>
        /// that is acceptable before a warning should be output. A value of 0 indicates that no warning should ever be output.
        /// </summary>
        public int TotalInMilliseconds { get; set; }

        #endregion


        private ValidationPerformanceThresholds Clone()
        {
            return (ValidationPerformanceThresholds) MemberwiseClone();
        }


        #region Class Members

        private static ValidationPerformanceThresholds _default;


        static ValidationPerformanceThresholds()
        {
            _default = new ValidationPerformanceThresholds
                           {IndividualValidatorInMilliseconds = 500, TotalInMilliseconds = 1000};
        }


        /// <summary>
        /// The default performance thresholds that should be used within the application
        /// </summary>
        /// <remarks>
        /// To change the defaults, call <see cref="SetDefault"/>
        /// </remarks>
        public static ValidationPerformanceThresholds Default
        {
            get { return _default.Clone(); }
        }

        public static ValidationPerformanceThresholds None
        {
            get { return new ValidationPerformanceThresholds(); }
        }


        public static void SetDefault(ValidationPerformanceThresholds value)
        {
            _default = value ?? None;
        }

        #endregion
    }
}