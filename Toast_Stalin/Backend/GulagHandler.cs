using Backend;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using System.Timers;

namespace Toast_Stalin.Backend {
    public class GulagHandler {
        private readonly CommandContext _commandContext;
        private readonly DiscordMember _discordMember;
        private readonly DiscordChannel _discordChannel;

        public static double GulagTimespan = 30000d;
        private const double StatusTimespan = 5000d;

        private static Timer StatusTimer { get; set; }
        private static Timer GulagTimer { get; set; }

        public GulagHandler(CommandContext commandContext, DiscordMember discordMember) {
            _commandContext = commandContext;
            _discordMember = discordMember;
            _discordChannel = commandContext.Guild.GetChannel(ConfigLoader.GulagChannelId);

            StatusTimer = new Timer(StatusTimespan) {AutoReset = true, Enabled = false};

            GulagTimer = new Timer(GulagTimespan) {AutoReset = false, Enabled = false};

            StatusTimer.Elapsed += StatusTimer_Elapsed;
            GulagTimer.Elapsed += GulagTimer_Elapsed;
        }

        public async Task StartGulagAsync() {
            StatusTimer.Start();
            GulagTimer.Start();

            await _commandContext.RespondAsync($"You are now gulaged {_discordMember.Mention}!");
        }

        private async void StatusTimer_Elapsed(object sender, ElapsedEventArgs e) {
            if (_discordMember.VoiceState == null) {
                return;
            }

            if (_discordMember.VoiceState.Channel.Id != _discordChannel.Id) {
                await _discordMember.PlaceInAsync(_discordChannel);
            }
        }

        private async void GulagTimer_Elapsed(object sender, ElapsedEventArgs e) {
            StatusTimer.Stop();
            GulagTimer.Stop();

            await _commandContext.RespondAsync($"You are not gulaged anymore {_discordMember.Mention}!");
        }
    }
}