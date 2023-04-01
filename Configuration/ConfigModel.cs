namespace Blaczko.Core.Configuration
{
	public abstract class ConfigModel
	{
		/// <summary>
		/// Custom init action
		/// </summary>
		public virtual void Init() { }

		/// <summary>
		/// Make sure that we have all the required fields
		/// </summary>
		internal void Validate()
		{
			// TODO: implement this
		}
	}
}
