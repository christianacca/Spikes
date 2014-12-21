using System;
using System.Data;
using System.Linq;
using Eca.Commons.Data.SelectQueryBuilder;

namespace Eca.Commons.Data
{
    /// <summary>
    /// Simple implementation for an <see cref="IIdGenerator{T}"/> that uses a database table 
    /// to maintain the current id
    /// </summary>
    public class IntegerDbTableIdGenerator : IIdGenerator<int>
    {
        #region Member Variables

        private const string GetNextIdProcName = "proc_Ids_GetNextId";
        private const string InsertSeedProcName = "proc_Ids_InsertSeed";
        private const string UpdateSeedProcName = "proc_Ids_UpdateSeed";
        private readonly string _connection;
        private readonly string _objectType;

        #endregion


        #region Constructors

        public IntegerDbTableIdGenerator(string connection, string objectType)
        {
            _connection = connection;
            _objectType = objectType;
        }

        #endregion


        #region IIdGenerator<int> Members

        public int NextId()
        {
            string sql = string.Format("EXEC dbo.{0} {1}, {2}",
                                       GetNextIdProcName,
                                       WhereStatement.FormatSqlValue(_objectType),
                                       WhereStatement.FormatSqlValue(1));

            DataTable result = SimpleDataAccess.ExecuteDataTable(command => {
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
            },
                                                                 _connection);

            DataRow row = result.AsEnumerable().Single();
            return row.Field<int>(0);
        }


        public int PeekNextId()
        {
            return 0;
        }


        public void Reseed(int nextId)
        {
            throw new NotImplementedException();
        }


        public void SaveChangesToId()
        {
            //nothing to save
        }

        #endregion


        #region Class Members

        /// <summary>
        /// Add security roles to the database objects that are used to serve up ids
        /// </summary>
        /// <param name="connection">Connection to the database containing the object to secure</param>
        /// <param name="roleNames">The names of security roles that should have permissions to generate ids</param>
        public static void AddDbPermissionsForGeneratingId(string connection, params string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                string sql = string.Format("GRANT EXECUTE ON OBJECT::[dbo].[{0}] TO [{1}]", GetNextIdProcName, roleName);
                SimpleDataAccess.ExecuteSql(sql, connection);
            }
        }


        /// <summary>
        /// Create the database objects required to generate identities
        /// </summary>
        /// <param name="connection">The database to create the objects in</param>
        /// <param name="deleteExistingTables">
        ///  Supply true if you want to delete an existing table created to store ids
        /// </param>
        /// <remarks>
        /// If the table for storing identities already exists in the database AND <paramref name="deleteExistingTables"/> 
        /// is <c>false</c>, then an exception will be thrown, and the existing table will not be deleted
        /// </remarks>
        public static void CreateDbObjects(string connection, bool deleteExistingTables)
        {
            var db = new SqlAdminQueries.DbInstance {ConnectionString = connection};

            DeleteDbObjects(connection, deleteExistingTables);
            CreateSeedTable(db);
            CreateSeedInsertStoredProc(db);
            CreateSeedUpdateStoredProc(db);
            CreateGetNextIdStoredProc(db);
        }


        private static void CreateGetNextIdStoredProc(SqlAdminQueries.DbInstance db)
        {
            string createGetNextIdProcSql =
                string.Format(
                    @"CREATE PROCEDURE [dbo].[{0}]
(
  @objectType AS VARCHAR(255),
  @blockSize AS INT = 1 
)
AS 
    SET NOCOUNT ON
    -- ensures even if command times out or batch is cancelled tx will rollback and locks released    
    SET XACT_ABORT ON
    
    DECLARE @isOwnerOfTransaction INT
    DECLARE @errmsg nvarchar(2048),
        @severity tinyint,
        @state TINYINT  
    DECLARE @resultId INT  
    
    BEGIN TRY
        IF @@TRANCOUNT = 0 
            BEGIN
                BEGIN TRAN
                SET @isOwnerOfTransaction = 1
            END
    
        -- sanitize bad inputs
        IF ( ISNULL(@blockSize, 0) = 0 ) 
            BEGIN
                SET @blockSize = 1
            END
        
        SELECT  @resultId = CurrentId + @blockSize
        FROM    dbo.tbl_Ids_Identities WITH (UPDLOCK)
        WHERE   ObjectType = @objectType
        
        UPDATE  dbo.tbl_Ids_Identities
        SET     CurrentId = @resultId
        WHERE   ObjectType = @objectType
        
        SELECT  @resultId AS NextId
        

        ExitLabel:
        IF @isOwnerOfTransaction = 1
            AND @@TRANCOUNT > 0 
            BEGIN
                COMMIT TRAN
            END
    END TRY
    BEGIN CATCH
        IF @isOwnerOfTransaction = 1
            AND @@TRANCOUNT > 0 
            BEGIN
                ROLLBACK TRAN
            END
        SELECT  @errmsg = error_message(),
                @severity = error_severity(),
                @state = error_state()        
        RAISERROR ( @errmsg, @severity, @state )
        RETURN -1
    END CATCH",
                    GetNextIdProcName);
            SimpleDataAccess.ExecuteSql(createGetNextIdProcSql, db.ConnectionString);
        }


        private static void CreateSeedInsertStoredProc(SqlAdminQueries.DbInstance db)
        {
            string createInsertSeedProcSql =
                string.Format(
                    @"CREATE PROC [dbo].[{0}]
(
  @objectType AS VARCHAR(255),
  @seed INT = 0
)
AS 
    INSERT  INTO dbo.tbl_Ids_Identities ( ObjectType, CurrentId )
    VALUES  ( @objectType, @seed )",
                    InsertSeedProcName);

            SimpleDataAccess.ExecuteSql(createInsertSeedProcSql, db.ConnectionString);
        }


        private static void CreateSeedTable(SqlAdminQueries.DbInstance db)
        {
            const string createTableSql =
                @"CREATE TABLE [dbo].[tbl_Ids_Identities](
	[ObjectType] [varchar](255) NOT NULL CONSTRAINT [PK_tbl_Ids_Identities] PRIMARY KEY CLUSTERED,
	[CurrentId] [int] NOT NULL,
)";
            SimpleDataAccess.ExecuteSql(createTableSql, db.ConnectionString);
        }


        private static void CreateSeedUpdateStoredProc(SqlAdminQueries.DbInstance db)
        {
            string createUpdateSeedProcSql =
                string.Format(
                    @"CREATE PROC [dbo].[{0}]
(
  @objectType AS VARCHAR(255),
  @seed INT
)
AS 
    UPDATE dbo.tbl_Ids_Identities SET CurrentId = @seed WHERE ObjectType = @objectType
    IF @@ROWCOUNT = 0 BEGIN
        RAISERROR('No rows were updated.', 16, 1)
    END",
                    UpdateSeedProcName);
            SimpleDataAccess.ExecuteSql(createUpdateSeedProcSql, db.ConnectionString);
        }


        /// <summary>
        /// Deletes the database objects that are created to manage id's
        /// </summary>
        /// <param name="connection">The database containing the objects</param>
        /// <param name="deleteExistingTables">
        ///  Supply true if you want to delete an existing table created to store ids
        /// </param>
        /// <remarks>
        /// This is safe to call even when if the database does not contain the objects
        /// </remarks>
        public static void DeleteDbObjects(string connection, bool deleteExistingTables)
        {
            var db = new SqlAdminQueries.DbInstance {ConnectionString = connection};
            db.DropStoredProc(GetNextIdProcName);
            db.DropStoredProc(InsertSeedProcName);
            db.DropStoredProc(UpdateSeedProcName);
            db.DropTable("tbl_Ids_Identities");
        }


        /// <summary>
        /// Inserts the initial seed record
        /// </summary>
        /// <param name="connection">The connection to the database</param>
        /// <param name="objectType">The name to associate with the seed</param>
        /// <param name="seed">The initial value for id to be generated</param>
        public static void InsertSeed(string connection, string objectType, int seed)
        {
            string sql = string.Format("EXEC dbo.{0} {1}, {2}",
                                       InsertSeedProcName,
                                       WhereStatement.FormatSqlValue(objectType),
                                       WhereStatement.FormatSqlValue(seed));
            SimpleDataAccess.ExecuteSql(sql, connection);
        }

        #endregion
    }
}