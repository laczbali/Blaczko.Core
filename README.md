# General extensions
### StringExtensions
Methods to clean and check strings, handle URLs, etc.

# General utils

### AsyncLock
Use with `using` to lock an asynchronous block of code.

### FileSystemUtil
Methods to handle files and directories.

### HttpClientWrapper
Simplifies the use of `HttpClient`.
Register it as a singleton, and use one of the `MakeRequestAsync` methods.

# Configuration handling
Use `AddServiceConfiguration` to register a a config model as a singleton.
The model will be bound from the configuration.
The config section name must match the model name (optionally the model name can have "Config" appended).

# SQLite helpers
Use the static `UsingDbAsync` and `UsingDbAsync<T>` methods from `DbUtil` to open a connection to a SQLite database and execute a query.
Both methods use `CreateConnectionAsync` to create a connection to the database.

If you instantiate `DbUtil`, you can use the `SafeUsingDbAsync` and `SafeUsingDbAsync<T>` methods,
which will lock the DB during use.

Use `ExecuteScriptAsync` from `SQLiteAsyncConnectionExtensions`, to execute a script on a SQLite database.
