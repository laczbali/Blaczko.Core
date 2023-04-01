using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blaczko.Core.Configuration
{
	public static class AppConfiguration
	{
		private static IConfiguration Configuration;

		public static IServiceCollection AddServiceConfiguration<T>(this IServiceCollection services, params string[] path)
			where T : class
		{
			var config = services.GetConfiguration();

			var sectionName = GetConfigSectionNameFromType<T>();
			var section = config.GetSection("OpenAiApi");

			var configurations = section.Get<T>(o => o.BindNonPublicProperties = true);;
			if (configurations is null)
			{
				throw new Exception($"Configuration was not found under {section}, for type {typeof(T).Name}");
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
