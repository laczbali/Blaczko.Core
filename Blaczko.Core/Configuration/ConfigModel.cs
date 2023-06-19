namespace Blaczko.Core.Configuration
{
	public abstract class ConfigModel
	{
		/// <summary>
		/// Custom init action, it will be called after configuration is binded
		/// </summary>
		public virtual void Init() { }
	}
}
