using SQLite;

namespace Blaczko.Core.Utils
{
    public static class DbUtil
    {
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

        public static async Task ExecuteScriptAsync(this SQLiteAsyncConnection db, string script, List<List<object>> parameters = null)
        {
            var snippets = script.Split(";", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if (parameters != null)
            {
                if (snippets.Length != parameters.Count)
                {
                    throw new ArgumentException("Number of parameters must be equal to number of snippets.");
                }
            }

            for (int i = 0; i < snippets.Length; i++)
            {
                var snippet = snippets[i];
                var parameter = parameters?[i];

                if (parameter is not null && parameter?.Count > 0)
                {
                    await db.ExecuteAsync(snippet, parameter.ToArray());
                }
                else
                {
                    await db.ExecuteAsync(snippet);
                }
            }
        }
    }
}
