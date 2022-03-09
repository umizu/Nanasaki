﻿using System.IO;
using System.Threading.Tasks;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nanasaki.Services;

namespace Nanasaki
{

	/// <summary>
	/// The entry point of the bot.
	/// </summary>
	public class Program
	{
		private static async Task Main()
		{
			var builder = new HostBuilder()
				.ConfigureAppConfiguration(x =>
				{
					var configuration = new ConfigurationBuilder()
						.SetBasePath(Directory.GetCurrentDirectory())
						.AddJsonFile("appsettings.json", false, true)
						.Build();

					x.AddConfiguration(configuration);
				})
				.ConfigureLogging(x =>
				{
					x.AddConsole();
					x.SetMinimumLevel(LogLevel.Debug);
				})
				.ConfigureDiscordHost<DiscordSocketClient>((context, config) =>
				{
					config.SocketConfig = new DiscordSocketConfig
					{
						LogLevel = Discord.LogSeverity.Debug,
						AlwaysDownloadUsers = false,
						MessageCacheSize = 200,
					};

					config.Token = context.Configuration["Token"];
				})
				.UseCommandService((context, config) =>
				{
					config.CaseSensitiveCommands = false;
					config.ThrowOnError = false;
					config.IgnoreExtraArgs = false;
					config.LogLevel = Discord.LogSeverity.Debug;
					config.DefaultRunMode = RunMode.Sync;
				})
				.ConfigureServices((context, services) =>
				{
					services.AddHostedService<CommandHandler>();
				})
				.UseConsoleLifetime();

			var host = builder.Build();
			using (host)
			{
				await host.RunAsync();
			}
		}
	}
}
