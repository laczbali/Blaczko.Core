using SQLite;

namespace Blaczko.Core.Utils
{
    public class DbUtil
    {
        public string DbFullPath { get; }

        private AsyncLock dbLock = new AsyncLock();

        public DbUtil(string dbFullPath)
        {
            DbFullPath = dbFullPath;
        }

        #region Instance methods

        public async Task<TResult> SafeUsingDbAsync<TResult>(Func<SQLiteAsyncConnection, Task<TResult>> action)
        {
            using (await dbLock.LockAsync())
            {
                return await DbUtil.UsingDbAsync<TResult>(this.DbFullPath, action);
            }
        }

        public async Task SafeUsingDbAsync(Func<SQLiteAsyncConnection, Task> action)
        {
            using (await dbLock.LockAsync())
            {
                await DbUtil.UsingDbAsync(this.DbFullPath, action);
            }
        }

        #endregion

        #region Static methods

        public static async Task<SQLiteAsyncConnection> CreateConnectionAsync(string dbFullPath)
        {
            FileSystemUtil.EnsureDir(Path.GetDirectoryName(dbFullPath));

            var connection = new SQLiteConnectionString(
                dbFullPath,
                SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex,
                storeDateTimeAsTicks: true);

            var db = new SQLiteAsyncConnection(connection);

            // Important for performance, see more here: https://blog.devart.com/increasing-sqlite-performance.html
            // Below settings is reset for every connection, therefore we have to set it again and again.
            await db.ExecuteScalarAsync<string>("pragma synchronous=off;");
            await db.ExecuteScalarAsync<string>("pragma journal_mode=off;");
            await db.ExecuteScalarAsync<string>("pragma temp_store=memory;");

            return db;
        }

        public static async Task<TResult> UsingDbAsync<TResult>(string dbPath, Func<SQLiteAsyncConnection, Task<TResult>> action)
        {
            var db = await CreateConnectionAsync(dbPath);
            try
            {
                return await action(db);
            }
            finally
            {
                await db.CloseAsync();
            }
        }

        public static async Task UsingDbAsync(string dbPath, Func<SQLiteAsyncConnection, Task> action)
        {
            var db = await CreateConnectionAsync(dbPath);
            try
            {
                await action(db);
            }
            finally
            {
                await db.CloseAsync();
            }
        }

        #endregion
    }
}
