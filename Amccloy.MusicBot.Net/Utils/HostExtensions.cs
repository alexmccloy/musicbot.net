using Amccloy.MusicBot.Net.Commands;
using Amccloy.MusicBot.Net.Trivia;
using Amccloy.MusicBot.Net.Trivia.MusicTrivia;
using Amccloy.MusicBot.Net.Trivia.TextTrivia;
using Microsoft.Extensions.DependencyInjection;

namespace Amccloy.MusicBot.Net.Utils;

public static class HostExtensions
{
    public static IServiceCollection AddDiscordCommand<T>(this IServiceCollection services) 
        where T : BaseDiscordCommand
    {
        return services.AddSingleton<BaseDiscordCommand, T>();
    }

    public static IServiceCollection AddTriviaQuestionProvider<T>(this IServiceCollection services) 
        where T : class, ITriviaQuestionProvider
    {
        return services.AddSingleton<ITriviaQuestionProvider, T>();
    }
    
    public static IServiceCollection AddMusicTriviaQuestionProvider<T>(this IServiceCollection services) 
        where T : class, IMusicTriviaQuestionProvider
    {
        return services.AddSingleton<IMusicTriviaQuestionProvider, T>();
    }
}