namespace Blaczko.Core.BaseClasses
{
    public abstract class ViewModel
    {
        [SQLite.Ignore]
        public virtual string ViewName { get => $"{this.GetType().Name}s"; }

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
