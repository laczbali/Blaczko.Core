namespace Blaczko.Core.SQLiteDb
{
    public abstract class ViewModelBase
    {
        [SQLite.Ignore]
        public virtual string ViewName { get => $"{GetType().Name}s"; }

        [SQLite.Ignore]
        public abstract string ViewScript { get; }

        [SQLite.Ignore]
        public string ViewGenQuery
        {
            get
            {
                return $"CREATE VIEW {ViewName} AS" + Environment.NewLine + ViewScript;
            }
        }
    }
}
