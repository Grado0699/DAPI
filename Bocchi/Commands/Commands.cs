using System.Text;
using Backend;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using ProxerProxy.Api;
using ProxerProxy.Model;

namespace Bocchi.Commands;

public class Commands : BaseCommandModule {
    private IAnimeApi _animeApi = new AnimeApi(ConfigLoader.ProxerProxyBasePath);
    private IEpisodeApi _episodeApi = new EpisodeApi(ConfigLoader.ProxerProxyBasePath);
    private IProxerConfigurationApi _proxerConfigurationApi = new ProxerConfigurationApi(ConfigLoader.ProxerProxyBasePath);

    [Command("glitch"), Aliases("g"), Description("Bocchi glitching")]
    public async Task Glitch(CommandContext ctx) {
        const string soundFile = "Resources/BocchiGlitches.ogg";

        var audioStreamer = new AudioStreamer(ctx.Client);
        await audioStreamer.PlaySoundFileAsync(soundFile, ctx.Member?.VoiceState.Channel);
    }

    [Command("leave"), Aliases("l"), Description("Leaves the current voice channel")]
    public async Task LeaveChannel(CommandContext ctx) {
        var voiceNextExt = ctx.Client.GetVoiceNext();
        var voiceConnection = voiceNextExt.GetConnection(ctx.Guild);

        voiceConnection?.Disconnect();

        await Task.CompletedTask;
    }

    [Command("config_get"), Description("Return the current proxer configuration")]
    public async Task GetProxerConfiguration(CommandContext ctx) {
        var proxerConfigurations = await _proxerConfigurationApi.GetAllProxerConfigurationsAsync();

        if (proxerConfigurations.Count == 0) {
            await ctx.RespondAsync("No proxer configurations found");
            return;
        }

        var stringBuilder = new StringBuilder();

        foreach (var proxerConfiguration in proxerConfigurations) {
            stringBuilder.Append($"Current token key is `{proxerConfiguration.TokenKey}` with value `{proxerConfiguration.TokenValue}`");
        }

        var discordMessageBuilder = new DiscordMessageBuilder();
        discordMessageBuilder.WithContent(stringBuilder.ToString());

        await ctx.RespondAsync(discordMessageBuilder);
    }

    [Command("config_set"), Description("Set a new proxer configuration using the format <key>:<value>")]
    public async Task SetProxerConfiguration(CommandContext ctx, [RemainingText] string tokenPair) {
        if (string.IsNullOrEmpty(tokenPair) || string.IsNullOrWhiteSpace(tokenPair)) {
            await ctx.RespondAsync("No token pair provided");
            return;
        }

        if (!tokenPair.Contains(':')) {
            await ctx.RespondAsync("Invalid token pair specified. Valid tokens are: `<key>:<value>`");
            return;
        }

        var key = tokenPair.Split(':')[0];
        var value = tokenPair.Split(':')[1];

        var proxerConfigurations = await _proxerConfigurationApi.GetAllProxerConfigurationsAsync();

        if (proxerConfigurations.Count > 1) {
            foreach (var proxerConfiguration in proxerConfigurations) {
                await _proxerConfigurationApi.DeleteProxerConfigurationAsync(proxerConfiguration.Id);
            }
        }

        if (proxerConfigurations.Count == 1) {
            var proxerConfiguration = proxerConfigurations.First();
            proxerConfiguration.TokenKey = key;
            proxerConfiguration.TokenValue = value;

            await _proxerConfigurationApi.UpdateProxerConfigurationAsync(proxerConfiguration);
        } else {
            var proxerConfiguration = new ProxerConfiguration {
                TokenKey = key,
                TokenValue = value
            };

            await _proxerConfigurationApi.AddProxerConfigurationAsync(proxerConfiguration);
        }

        await ctx.RespondAsync($"Successfully added new proxer configuration with token:\n- key: `{key}`\n- value: `{value}`");
    }

    [Command("anime_list"), Description("List all stored animes, with their current state")]
    public async Task ListAnime(CommandContext ctx) {
        var animeList = await _animeApi.GetAllAnimeAsync();

        if (animeList.Count == 0) {
            await ctx.RespondAsync("There are currently no anime stored");
            return;
        }
        
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("```");
        stringBuilder.Append("Proxer ID \t\t State \t\t Title\n");


        foreach (var anime in animeList) {
            stringBuilder.Append($"{anime.ProxerId} \t\t {anime.State} \t\t {anime.Title}\n");
        }

        stringBuilder.Append("```");

        var discordMessageBuilder = new DiscordMessageBuilder();
        discordMessageBuilder.WithContent(stringBuilder.ToString());

        await ctx.RespondAsync(discordMessageBuilder);
    }

    [Command("anime_add"),
     Description("Add a new anime to the database. Open the details page on Proxer `https://proxer.me/info/<ID>#top` and copy the ID into the command." + "Also provide the `anime title` and its `total episodes`." +
                 "Do this in a comma-separated list, with order:\n- `title`\n- `ID`\n- `total episodes`")]
    public async Task AddAnime(CommandContext ctx, [RemainingText] string parameters) {
        if (string.IsNullOrEmpty(parameters) || string.IsNullOrWhiteSpace(parameters)) {
            await ctx.RespondAsync("No parameters provided");
            return;
        }

        if (!parameters.Contains(',')) {
            await ctx.RespondAsync("No comma separated lists provided");
            return;
        }

        var parameterList = parameters.Split(',').ToList();

        if (parameterList.Count != 3) {
            await ctx.RespondAsync("Invalid number of parameters provided");
            return;
        }

        var animeId = int.Parse(parameterList[1]);
        var totalEpisodes = int.Parse(parameterList[2]);

        var anime = new Anime {
            Title = parameterList[0],
            ProxerId = animeId,
            TotalEpisodes = totalEpisodes,
            State = Anime.StateEnum.New
        };

        anime = await _animeApi.AddAnimeAsync(anime);

        await ctx.RespondAsync($"Successfully added anime to database. Assigned ID: `{anime.Id}`");
    }

    [Command("anime_start_query"), Description("Query Proxer to extract all episode video URLs for the given anime. Use the `Proxer ID`!")]
    public async Task QueryEpisodes(CommandContext ctx, [RemainingText] string proxerIdString) {
        if (string.IsNullOrEmpty(proxerIdString) || string.IsNullOrWhiteSpace(proxerIdString)) {
            await ctx.RespondAsync("No Proxer ID provided");
            return;
        }

        if (!int.TryParse(proxerIdString, out int proxerId)) {
            await ctx.RespondAsync("Invalid proxer ID provided, could not parse as integer");
            return;
        }
        
        var animeList = await _animeApi.GetAllAnimeAsync();
        var anime = animeList.FirstOrDefault(x => x.ProxerId == proxerId);
    
        if (anime is null) {
            await ctx.RespondAsync($"No anime found with Proxer ID `{proxerId}`");
            return;
        }
        
        await _animeApi.UpdateAnimeStateAsync(anime.Id, nameof(Anime.StateEnum.Queued).ToLower());

        await ctx.RespondAsync($"Anime with Proxer ID `{proxerId}` updated to `{Anime.StateEnum.Queued.ToString()}`");
    }

    [Command("anime_clear"), Description("Removes all video URLs for the given anime. Use the `Proxer ID`!")]
    public async Task ClearEpisodesOfAnime(CommandContext ctx, [RemainingText] string proxerIdString) {
        if (string.IsNullOrEmpty(proxerIdString) || string.IsNullOrWhiteSpace(proxerIdString)) {
            await ctx.RespondAsync("No Proxer ID provided");
            return;
        }

        if (!int.TryParse(proxerIdString, out int proxerId)) {
            await ctx.RespondAsync("Invalid proxer ID provided, could not parse as integer");
            return;
        }

        var animeList = await _animeApi.GetAllAnimeAsync();
        var anime = animeList.FirstOrDefault(x => x.ProxerId == proxerId);

        if (anime is null) {
            await ctx.RespondAsync($"No anime found with Proxer ID `{proxerId}`");
            return;
        }

        var episodeList = await _episodeApi.GetEpisodesByAnimeIdAsync(anime.Id);

        foreach (var episode in episodeList) {
            await _episodeApi.DeleteEpisodeAsync(anime.Id, episode.VarEpisode.ToString());
        }
        
        await _animeApi.UpdateAnimeStateAsync(anime.Id, nameof(Anime.StateEnum.New).ToLower());

        await ctx.RespondAsync($"Successfully deleted all episodes of anime with Proxer ID `{proxerId}`");
    }

    [Command("episode_list"), Description("List all episodes of an anime. Use the `Proxer ID`!")]
    public async Task ListEpisodes(CommandContext ctx, [RemainingText] string proxerIdString) {
        if (string.IsNullOrEmpty(proxerIdString) || string.IsNullOrWhiteSpace(proxerIdString)) {
            await ctx.RespondAsync("No Proxer ID provided");
            return;
        }

        if (!int.TryParse(proxerIdString, out int proxerId)) {
            await ctx.RespondAsync("Invalid proxer ID provided, could not parse as integer");
            return;
        }

        var animeList = await _animeApi.GetAllAnimeAsync();
        var anime = animeList.FirstOrDefault(x => x.ProxerId == proxerId);

        if (anime is null) {
            await ctx.RespondAsync($"No anime found with Proxer ID `{proxerId}`");
            return;
        }

        var episodeList = await _episodeApi.GetEpisodesByAnimeIdAsync(anime.Id);
        
        if (episodeList.Count == 0) {
            await ctx.RespondAsync("This anime has not been queried yet");
            return;
        }
        
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("```");
        stringBuilder.Append("Episode number \t\t URL\n");

        foreach (var episode in episodeList) {
            stringBuilder.Append($"{episode.VarEpisode} \t\t {episode.VideoUrl}\n");
        }

        stringBuilder.Append("```");

        var discordMessageBuilder = new DiscordMessageBuilder();
        discordMessageBuilder.WithContent(stringBuilder.ToString());

        await ctx.RespondAsync(discordMessageBuilder);
    }
}