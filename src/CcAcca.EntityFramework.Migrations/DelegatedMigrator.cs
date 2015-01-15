using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Migrations;

namespace CcAcca.EntityFramework.Migrations
{
    /// <summary>
    /// A Migrator which delegates the work of running migrations to another implementation
    /// </summary>
    public class DelegatedMigrator: IDisposable
    {
        private readonly Func<IEnumerable<string>> _getPendingMigrationsImpl;
        private readonly Func<IEnumerable<string>> _getGetDatabaseMigrationsImpl;
        private readonly Action<string> _updateImpl;
        private readonly DbConnection _connection;
        private readonly Action _disposeImpl;

        public DelegatedMigrator(Func<IEnumerable<string>> getPendingMigrationsImpl,
            Func<IEnumerable<string>> getGetDatabaseMigrationsImpl,
            Action<string> updateImpl,
            DbConnection connection,
            Action dispose = null)
        {
            _getPendingMigrationsImpl = getPendingMigrationsImpl;
            _getGetDatabaseMigrationsImpl = getGetDatabaseMigrationsImpl;
            _updateImpl = updateImpl;
            _connection = connection;
            _disposeImpl = dispose ?? (() =>
            {
                // noop
            });
        }

        /// <summary>
        /// The type name of the <see cref="DbMigrationsConfiguration"/> that defines the migrations to run
        /// </summary>
        public string ConfigurationTypeName { get; set; }

        /// <summary>
        /// Returns the names of the migrations that have been applied to the target database
        /// </summary>
        public IEnumerable<string> GetDatabaseMigrations()
        {
            return _getGetDatabaseMigrationsImpl();
        }

        /// <summary>
        /// Returns the names of the pending migrations outstanding against the database
        /// </summary>
        public IEnumerable<string> GetPendingMigrations()
        {
            return _getPendingMigrationsImpl();
        }

        /// <summary>
        /// Derived from <see cref="DbMigrationsConfiguration.AutomaticMigrationsEnabled"/>
        /// </summary>
        public bool IsAutoMigrationsEnabled { get; set; }

        /// <summary>
        /// Determines the order in which a migration from this migrator should run when there
        /// is a tie on the <see cref="MigrationInfo.CreatedOn"/> date of a collection of
        /// migrations
        /// </summary>
        public int Priority { get; set; }


        private string GetMigrationHistoryInsertSql()
        {
            // todo: generate this by using a script only migration
            return @"INSERT [Base].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
VALUES (N'201501032326177_Rename LookupItem pk', N'Spikes.Migrations.BaseData.SpikesMigrationsBaseDb',  0x1F8B0800000000000400ED5A5F6FDB36107F1FB0EF20E8A91D52CB495EBAC06E913AC9602C4E8A3829F65630D2D9212A51AA480536867DB23DEC23ED2BECA8FF222559FE97B8C050A04848DE91BCFBFD4EC7BBFCFBF73F838F0BCF359E21E4D46743F3B8D7370D60B6EF50361F9A9198BD7B6F7EFCF0F34F834BC75B185FB275A7721D4A323E349F8408CE2C8BDB4FE011DEF3A81DFADC9F899EED7B16717CEBA4DFFFD53A3EB6005598A8CB3006771113D483F817FC75E4331B02111177E23BE0F2741C67A6B156E38678C00362C3D09C06F41BF0DE84CE4322F030BCF78970B820821443A671EE5282679B823B330DC2982FE299B3070E5311FA6C3E0D7080B8F7CB0070DD8CB81CD21B9D15CBBB5EAE7F222F671582992A3BE2C2F7D654787C9A5ACB52C537B2B9995B13ED798976174B79EBD8A643F3DAF7BF45C15880671AEA76672337944B9B8C1E3BAB576838325AD61DE5E84190C97F47C628724514C290412442822B3E478F2EB57F87E5BDFF0DD89045AE5B3E3D9E1FE72A0338F439F40308C5F20E66E99D7091695855414B95CCE5CA42C96DC74C9C9E98C60D6E4F1E5DC8E151B2CC54F821FC060CF09EE07C26424088DE1D3B101B58DB5ED96C8416C976433C22D94C634216D7C0E6E20969D8477A5DD10538D9487A820746919B2824C2086A4ED8BEEB05703BA44182A497DE3C8589A3185911BA21CF741E9BB856DC34EEC04D90F5448384DF2900BF163844FE5F85BE77E7BBB96079F6EB3D09E720F0207EE392A91F85B672A78155B0A703A7B6E3D3A170493A6C5D2A694EDE2393E4FFAFC024CA03972CAF28B84ECBEEA7DD365F8F0329C27741840CE52D44C8B8B21111F06B1B4ACD9B522193FF9F0CDDC8D0E11BB6159BF0C7B501DD192C3285DB1630651D7B018D2135A4E79094CB0FDC1E2EF05872951C7E2DF35E0191D7DBD6C28A9A0332727AB217B6F339E7BE4DE39B57A274250A57CF7DC91C6375482EAE9FA51313B41D0DD05A788CA1D9EFF58E353AB7AACED31E557592FD57D5FFA2E9C63008A18C43C4C5F71A47F75126F49849994D03E2AEBCA122D931DE4A47E47BA83317100093A172A50DBA6C5E24ACFA11F29D948FC12A1B0DAC125E74BAA28C400908D32324DC2AA815BF741F6BA88B0C49D9CB53F0AAB890EAA720EA328902D835A8D00056A7A859C94A0519B7EB5414B14A515232A27A946AF6535AD89C23A9DEEDC4CFFC1E251B6838E944474D537A7635D1A85EBA2634E5E829CA2756523FC9EA2C5643A1653021418051B25478494730BCC75597D1BBE9FAC5072FD161D9BCA606919F36DF09731F32076536792B5FD1900BF9157D24324E8F1C4F5BD6C09506F4659BEA74D07D98A13293913F97F9D95E8B2AB3490F3BA9CE2BBCB92703579C00D62242178ECB63C425615D1638F2DDC8638DF59036F9A4385156908C74D750293494155526BAEB2B4271595973809650572CAB8570CDAF0ADD54A8AC01A4BD8168630075078F6AE47AF3364927EF87B27C32B206742A2FEB0A762A3307E3F2E203B68ED3C75CFE7C3B7BD3C9FBD91E6F3700407DF2BC3F086C1B7DBA43685B87EF82979547EBE13BA7FA10ADB0AB3253AB113FB60E8D1F3A5F881B692F7685AB18E73DCA085EF8507DA7BE880FDF7DDA13B7AC4A9BECE644AD2EF0C3F9F1301DA819FA952CDCFA59D39E0EEA927CF7FC09A13C150669DABEBA71ABE5F1C912D3400B3F5327CEE1971C33DD38A8F6A6DFDD914BF1BEC582096174065C24A522139F19EF954EEFE1745D2DCE1D779BD66B3C90157EA700CED0BCD71FF62FD20FA5D2092B4BD36B969ACB2D50F64C42FB89847AEB66FB0EE7CE74AB0DCCD82A9ADFC6CC81C5D0FC33163A33C67F646F7EE7C8B80D11C46746DFF8ABBCF91635DDE646E30ED1B35DD3632FD829B72976879D9A9E5EADF2D3FE8EDCD750D7FE51BDA2468C0EDC40912363CC1F18FD1EE1888468951C1BC3E18D47166FDBFDD4AD4FB395C2869EC47687AC660C6D2C3879BFC2967BEC6AECAA85917C1F5FBE67F1720D8A86D2E0417624F4626CD746C38A3E43920C0ECD4F7181F73E416C5B3DBC7EA7964644CB0EABB517C97B4B97A276875283E355BB18358D860E1D8BB2E7D482F45E3A14FA7302315CFA6B51E410A7F342857C6731B02BE8CDD78CD9CCCF68A49C285BA204D70908E220B4CF434167C416386D03E77107397DC75D7A8FE08CD96D248248E095C17B742BF9B52463DBFE711BA67AE6C16D9CB3F25D5C018F49F10A70CB3E4554A632D943BF26FC37A8902C4F1303E94B211384F932D774E3B38E8A52F3E5C1E91E3CCCB104F05B3625CFB0C9D9904FD73027F6327B15362B59ED88AAD9071794CC43E2F15447218FBF22861D6FF1E13FB5FA1FCE342D0000 , N'6.1.1-30610')";
        }

        /// <summary>
        ///     Insert only the snapshot of the model into the migration history table
        /// </summary>
        /// <param name="migrationName">The target migration whose model snapshot is to be inserted</param>
        /// <param name="previousMigrationName">
        ///     Previous migration required in order to create an sql script of just the target
        ///     migration
        /// </param>
        public void InsertMigrationHistory(string migrationName, string previousMigrationName)
        {
            bool mustClose = _connection.State != ConnectionState.Open;
            try
            {
                _connection.Open();

                DbCommand cmd = _connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = GetMigrationHistoryInsertSql();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (mustClose) _connection.Close();
            }
        }

        /// <summary>
        /// Runs the migrations against the <see cref="DbMigrationsConfiguration.TargetDatabase"/>
        /// </summary>
        public void Update(string targetMigration)
        {
            _updateImpl(targetMigration);
        }


        public override string ToString()
        {
            return String.Format("ConfigurationTypeName: {0}, Priority: {1}", ConfigurationTypeName, Priority);
        }

        public void Dispose()
        {
            _disposeImpl();
        }
    }
}