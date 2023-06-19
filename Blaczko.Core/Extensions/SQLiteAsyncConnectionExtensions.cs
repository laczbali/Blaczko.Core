using SQLite;

namespace Blaczko.Core.Extensions
{
    public static class SQLiteAsyncConnectionExtensions
    {
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
