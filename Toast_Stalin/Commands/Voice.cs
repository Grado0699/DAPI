using System;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.VoiceNext;
using Toast_Stalin.Backend;

namespace Toast_Stalin.Commands
{
    public class Voice : BaseCommandModule
    {
        [Command("supreme"), Aliases("-sp"), Description("Slava!")]
        public async Task Supreme(CommandContext ctx)
        {
            const string RessourceName = "Ressources/Supreme.mp3";

            if (!RessourceChecker.IsRessourceAvailable(RessourceName))
            {
                ctx.Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"Ressource {RessourceName} is missing.", DateTime.Now);
                return;
            }

            ctx.Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Ressource {RessourceName} is present.", DateTime.Now);

            await DiscordStreamer.StreamToDiscord(ctx, RessourceName);
        }

        [Command("supreme+"), Aliases("-sp+"), Description("Bogatyye!")]
        public async Task SupremePlus(CommandContext ctx)
        {
            const string RessourceName = "Ressources/SupremePlus.mp3";

            if (!RessourceChecker.IsRessourceAvailable(RessourceName))
            {
                ctx.Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"Ressource {RessourceName} is missing.", DateTime.Now);
                return;
            }

            ctx.Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Ressource {RessourceName} is present.", DateTime.Now);

            await DiscordStreamer.StreamToDiscord(ctx, RessourceName);
        }

        [Command("exasupreme"), Aliases("-es"), Description("Moshchnost'!")]
        public async Task ExaSupreme(CommandContext ctx)
        {
            const string RessourceName = "Ressources/ExaSupreme.mp3";

            if (!RessourceChecker.IsRessourceAvailable(RessourceName))
            {
                ctx.Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"Ressource {RessourceName} is missing.", DateTime.Now);
                return;
            }

            ctx.Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Ressource {RessourceName} is present.", DateTime.Now);

            await DiscordStreamer.StreamToDiscord(ctx, RessourceName);
        }

        [Command("boris"), Aliases("-b"), Description("Moya lyubimaya muzyka, Boris!")]
        public async Task Hardbass(CommandContext ctx)
        {
            const string RessourceName = "Ressources/Boris.mp3";

            if (!RessourceChecker.IsRessourceAvailable(RessourceName))
            {
                ctx.Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, $"Ressource {RessourceName} is missing.", DateTime.Now);
                return;
            }

            ctx.Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Ressource {RessourceName} is present.", DateTime.Now);

            await DiscordStreamer.StreamToDiscord(ctx, RessourceName);
        }

        [Command("leave"), Aliases("-l"), Description("Pokinut' kanal!")]
        public async Task LeaveChannel(CommandContext ctx)
        {
            var VoiceNextExt = ctx.Client.GetVoiceNext();
            var VoiceConnection = VoiceNextExt.GetConnection(ctx.Guild);

            if (VoiceConnection == null)
            {
                await ctx.RespondAsync($"There is currently no voice connection up. Into Goulag with you!");
                ctx.Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"There is currently no voice connection up.", DateTime.Now);
                return;
            }

            VoiceConnection.Disconnect();
            ctx.Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Successfully closed connection to voice channel.", DateTime.Now);
        }
    }
}
