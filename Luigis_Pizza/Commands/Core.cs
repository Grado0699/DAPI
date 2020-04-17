using System;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;

namespace Luigis_Pizza.Commands
{
    public class Core : BaseCommandModule
    {
        public static double TimerSpan = 2700000d;

        public static bool EnableTimer = true;

        [Command("toggletimer"), Aliases("-tt"), Description("This command enables/disables the timer. It also uses interactity with a configured timespan of 15 seconds.")]
        public async Task ToggleTimer(CommandContext ctx)
        {
            var InterActExt = ctx.Client.GetInteractivity();

            await ctx.RespondAsync($"With this command you can enable `(true)` or disable `(false)` the timer. Current status is `{EnableTimer}`.");

            var Message = await InterActExt.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id, TimeSpan.FromMinutes(1));

            if (Message.Result == null)
            {
                ctx.Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, "Command 'toggletimer': The reply of the message is null. The interactivity probably run into timeout.", DateTime.Now);
                await ctx.RespondAsync("Timeout reached. Run the command again.");
                return;
            }

            if (Message.Result.Content.ToLower().Contains("true"))
            {
                EnableTimer = true;

                await ctx.RespondAsync($"Status of timer successfully set to: {EnableTimer}");
                ctx.Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Command 'toggletimer': Status of timer successfully set to 'true'.", DateTime.Now);
            }
            else
            {
                if (Message.Result.Content.ToLower().Contains("false"))
                {
                    EnableTimer = false;

                    await ctx.RespondAsync($"Status of timer successfully set to: {EnableTimer}");
                    ctx.Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, "Command 'toggletimer': Status of timer successfully set to 'false'.", DateTime.Now);
                }
                else
                {
                    await ctx.RespondAsync("The input is invalid. Run the command again and use either `true` to enable or `false` to disable the timer.");
                    ctx.Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, "Command 'toggletimer': Invalid reply received.", DateTime.Now);
                }
            }
        }

        [Command("settime"), Aliases("-st"), Description("This command sets a new timespan for the timer. It also uses interactity with a configured timespan of 15 seconds.")]
        public async Task SetTimeSpan(CommandContext ctx)
        {
            var InterActExt = ctx.Client.GetInteractivity();

            await ctx.RespondAsync($"With this command you can set a new timespan `(in X miliseconds)` for the timer. Current timespan is `{TimerSpan}`.");

            var Message = await InterActExt.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id, TimeSpan.FromMinutes(1));

            if (Message.Result == null)
            {
                ctx.Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, "Command 'settime': The reply of the message is null. The interactivity probably run into timeout.", DateTime.Now);
                await ctx.RespondAsync("Timeout reached. Run the command again.");
                return;
            }

            if (Double.TryParse(Message.Result.Content, out TimerSpan))
            {
                await ctx.RespondAsync($"New timespan successfully set to: {TimerSpan}");
                ctx.Client.DebugLogger.LogMessage(LogLevel.Info, Assembly.GetExecutingAssembly().GetName().Name, $"Command 'settime': New timespan successfully set to '{TimerSpan}'.", DateTime.Now);
            }
            else
            {
                TimerSpan = 10000d;

                await ctx.RespondAsync($"The input is invalid. Setting the timerspan to its default value: {TimerSpan}");
                ctx.Client.DebugLogger.LogMessage(LogLevel.Error, Assembly.GetExecutingAssembly().GetName().Name, "Command 'settime': Input is invalid. Setting the timespan to its default value.", DateTime.Now);
            }
        }
    }
}
