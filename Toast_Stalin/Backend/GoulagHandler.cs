using System.Threading.Tasks;
using System.Timers;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace Toast_Stalin.Backend
{
    public class GoulagHandler
    {
        private const double IntervalCheckIsUserInGoulag = 5000d;
        private static Timer CheckEachSecondIsUserInGoulag { get; set; }
        private static Timer TotalTimeUserIsGoulaged { get; set; }
        private static CommandContext Context { get; set; }
        private static DiscordMember GuildMemberToGoulag { get; set; }
        private static DiscordChannel GoulagChannel { get; set; }

        public GoulagHandler(CommandContext ctx, DiscordMember UserToGoulag)
        {
            CheckEachSecondIsUserInGoulag = new Timer(IntervalCheckIsUserInGoulag)
            {
                AutoReset = true
            };

            TotalTimeUserIsGoulaged = new Timer(ConfigLoader.TimeUserIsGoulaged)
            {
                AutoReset = false
            };

            CheckEachSecondIsUserInGoulag.Elapsed += CheckEachSecondIsUserInGoulag_Elapsed;
            TotalTimeUserIsGoulaged.Elapsed += TotalTimeUserIsGoulaged_Elapsed;

            Context = ctx;
            GuildMemberToGoulag = UserToGoulag;
            GoulagChannel = ctx.Guild.GetChannel(ConfigLoader.GoulagChannelId);
        }

        public async Task StartGoulagAsync()
        {
            CheckEachSecondIsUserInGoulag.Start();
            TotalTimeUserIsGoulaged.Start();

            await Context.RespondAsync($"You are now goulaged {GuildMemberToGoulag.Mention}!");
        }

        private async void CheckEachSecondIsUserInGoulag_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(GuildMemberToGoulag.VoiceState == null)
            {
                return;
            }

            if(GuildMemberToGoulag.VoiceState.Channel.Id != GoulagChannel.Id)
            {
                await GuildMemberToGoulag.PlaceInAsync(GoulagChannel);
            }
        }

        private async void TotalTimeUserIsGoulaged_Elapsed(object sender, ElapsedEventArgs e)
        {
            CheckEachSecondIsUserInGoulag.Stop();
            TotalTimeUserIsGoulaged.Stop();

            await Context.RespondAsync($"You are not goulaged anymore {GuildMemberToGoulag.Mention}!");
        }
    }
}
