using Amccloy.MusicBot.Net.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Amccloy.MusicBot.Net.Utils;

public static class HostExtensions
{
    public static IServiceCollection AddDiscordCommand<T>(this IServiceCollection services) where T : BaseDiscordCommand
    {
        return services.AddSingleton<BaseDiscordCommand, T>();
    }
}