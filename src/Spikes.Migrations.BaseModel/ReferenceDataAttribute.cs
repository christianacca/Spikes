using System;

namespace Spikes.Migrations.BaseModel
{
    /// <summary>
    ///     Used to annotate a model class indicating that instances are reference data as opposed
    ///     to transactional data
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ReferenceDataAttribute : Attribute
    {
    }
}