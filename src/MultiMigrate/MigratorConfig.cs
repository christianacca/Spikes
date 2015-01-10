namespace MultiMigrate
{
    public class MigratorConfig
    {
        /// <summary>
        /// Specifies the name of the assembly that contains the migrations configuration type.
        /// </summary>
        public string Assembly { get; set; }

        /// <summary>
        ///     Specifies the name of the migrations configuration type. If omitted, Code First Migrations will attempt to locate a
        ///     single migrations configuration type in the specified assembly.
        /// </summary>
        public string ConfigurationType { get; set; }

        /// <summary>
        ///     Specifies the name of the assembly that contains the DbContext type if different from the assembly that contains
        ///     the migrations configuration type
        /// </summary>
        public string ContextAssembly { get; set; }

        /// <summary>
        ///     Must be set to the same value as AutomaticMigrationsEnabled property of the corrosponding migrations configuration
        /// </summary>
        public bool AutomaticMigrationsEnabled { get; set; }
    }
}