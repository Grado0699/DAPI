﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System;
using System.Threading.Tasks;

namespace Luigis_Pizza.Commands
{
    public class Core : BaseCommandModule
    {
        public static double TimerSpan = 2700000d;
        public static bool EnableTimer = false;

        [Command("toggletimer"), Aliases("-tt"), Description("This command enables/disables the timer. It also uses interactity with a configured timespan of 15 seconds.")]
        public async Task ToggleTimer(CommandContext ctx)
        {
            var InterActExt = ctx.Client.GetInteractivity();

            await ctx.RespondAsync($"With this command you can enable `(true)` or disable `(false)` the timer. Current status is `{EnableTimer}`.");

            var Message = await InterActExt.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id, TimeSpan.FromMinutes(1));

            if (Message.Result == null)
            {
                await ctx.RespondAsync("Timeout reached. Run the command again.");
                return;
            }

            if (Message.Result.Content.ToLower().Contains("true"))
            {
                EnableTimer = true;
                await ctx.RespondAsync($"Status of timer successfully set to: {EnableTimer}");
            }
            else
            {
                if (Message.Result.Content.ToLower().Contains("false"))
                {
                    EnableTimer = false;
                    await ctx.RespondAsync($"Status of timer successfully set to: {EnableTimer}");
                }
                else
                {
                    await ctx.RespondAsync("The input is invalid. Run the command again and use either `true` to enable or `false` to disable the timer.");
                }
            }
        }

        [Command("settime"), Aliases("-st"), Description("This command sets a new timespan for the timer. It also uses interactity with a configured timespan of 15 seconds.")]
        public async Task SetTimeSpan(CommandContext ctx)
        {
            var InterActExt = ctx.Client.GetInteractivity();
            var Message = await InterActExt.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id, TimeSpan.FromMinutes(1));

            if (Message.Result == null)
            {
                await ctx.RespondAsync("Timeout reached. Run the command again.");
                return;
            }

            if (double.TryParse(Message.Result.Content, out TimerSpan))
            {
                await ctx.RespondAsync($"New timespan successfully set to: {TimerSpan}");
            }
            else
            {
                TimerSpan = 10000d;
                await ctx.RespondAsync($"The input is invalid. Setting the timerspan to its default value: {TimerSpan}");
            }
        }
    }
}
