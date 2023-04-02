using Blaczko.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blaczko.Core.Configuration
{
	public static class AppConfiguration
	{
		private static IConfiguration Configuration;

		/// <summary>
		/// It will attempt to bind configurations to the provided type.<br/>
		/// The configuration section is expected to be in root, with the name of the type.<br/>
		/// If the type has a Config suffix, it will be removed from the section name.<br/>
		/// <br/>
		/// Validation will be performed, that all properties with <see cref="RequiredKeyAttribute"/> are present (and not null) in the configuration.
		/// An exception will be thrown, if a required key is not found.<br/>
		/// <br/>
		/// If a config class inherits from <see cref="ConfigModel"/>, it will be initialized after binding.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="services"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static IServiceCollection AddServiceConfiguration<T>(this IServiceCollection services, params string[] path)
			where T : class
		{
			var config = services.GetConfiguration();

			var sectionName = GetConfigSectionNameFromType<T>();
			var section = config.GetSection("OpenAiApi");
			var sectionChildren = section.GetChildren();

			var requiredProps = typeof(T).GetProperties().Where(p => Attribute.IsDefined(p, typeof(RequiredKeyAttribute)));
			foreach (var prop in requiredProps)
			{
				var settingsVal = sectionChildren.FirstOrDefault(x => x.Key == prop.Name);
				if (settingsVal is null || settingsVal?.Value.NullIfEmpty() is null)
				{
					throw new Exception($"Configuration was not found under {sectionName}, for type {typeof(T).Name}, property {prop.Name}");
				}
			}

			var configurations = section.Get<T>(o => o.BindNonPublicProperties = true);;
			if (configurations is null)
			{
				throw new Exception($"Configuration was not found under {sectionName}, for type {typeof(T).Name}");
			}

			if(configurations is ConfigModel commonConfig)
			{
				commonConfig.Init();
			}

			services.AddSingleton(configurations);

			return services;
		}

		public static string GetConfigSectionNameFromType<T>()
		{
			const string ConfigurationTypeSuffix = "Config";

			string sectionName = typeof(T).Name;
			if (sectionName.EndsWith(ConfigurationTypeSuffix))
			{
				sectionName = sectionName.Substring(0, sectionName.Length - ConfigurationTypeSuffix.Length);
			}

			return sectionName;
		}

		private static IConfiguration GetConfiguration(this IServiceCollection services)
		{
			if (Configuration is not null) return Configuration;

			var serviceProvider = services.BuildServiceProvider();
			Configuration = serviceProvider.GetService<IConfiguration>();
			return Configuration;
		}
	}
}
