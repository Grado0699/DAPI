using System;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;

namespace Gengbleng.Commands
{
    public class Core : BaseCommandModule
    {
        //public static double TimerSpan = 900000d;
        public static double TimerSpan = 5000d;
        public static bool EnableTimer = true;
    }
}
