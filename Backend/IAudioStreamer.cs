using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace Backend
{
    public interface IAudioStreamer
    {
        Task PlaySoundFileAsync(string soundFile, DiscordChannel voiceChannel, string volume = "100");
    }
}