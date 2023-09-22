using SQLite;

namespace Blaczko.Core.SQLiteDb
{
    public static class SQLiteAsyncConnectionExtensions
    {
        /// <summary>
        /// Execute a script with parameters.
        /// </summary>
        /// <param name="db">DB to execute the script on</param>
        /// <param name="script">Script will be split to snippets on ';' and snippets will be executed separately.</param>
        /// <param name="parameters">
        ///     Outer list represents the idividual snippets.<br/>
        ///     Inner list represents the parameters for the snippet.<br/>
        ///     Length of outer list must be equal to length of snippets, if not null.
        /// </param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task ExecuteScriptAsync(this SQLiteAsyncConnection db, string script, List<List<object>> parameters = null)
        {
            var snippets = script.Split(";", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if (parameters is not null)
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
