using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bunta.Backend
{
    public static class SongParser
    {
        /// <summary>
        /// Removes all paths for songs and wraps all entries of a string array into a single message.
        /// </summary>
        /// <param name="ListOfSongs"></param>
        /// <returns></returns>
        public static Task<string> ParseSongsAsync(string[] ListOfSongs)
        {
            int CountSongs = 1;
            string MessageListOfSongs = "```";

            foreach (var Song in ListOfSongs)
            {
                MessageListOfSongs += CountSongs.ToString() + ". " + Song.Substring(Song.IndexOf("\\") + 1, Song.Length - (Song.IndexOf("\\") + 1) - 4) + "\r\n";
                CountSongs++;
            }

            return Task.FromResult(MessageListOfSongs += "```");
        }

        /// <summary>
        /// Removes all paths for songs and wraps all entries off a string queue into a single message.
        /// </summary>
        /// <param name="Playlist"></param>
        /// <returns></returns>
        public static Task<string> ParseSongsAsync(Queue<string> Playlist)
        {
            int CountSongs = 1;
            string MessageListOfSongs = "```";

            foreach (var Song in Playlist)
            {
                MessageListOfSongs += CountSongs.ToString() + ". " + Song.Substring(Song.IndexOf("\\") + 1, Song.Length - (Song.IndexOf("\\") + 1) - 4) + "\r\n";
                CountSongs++;
            }

            return Task.FromResult(MessageListOfSongs += "```");
        }

        /// <summary>
        /// Removes all paths of a song.
        /// </summary>
        /// <param name="Song"></param>
        /// <returns></returns>
        public static Task<string> ParseSongsAsync(string Song)
        {
            string MessageListOfSongs = "`" + Song.Substring(Song.IndexOf("\\") + 1, Song.Length - (Song.IndexOf("\\") + 1) - 4) + "`";
            return Task.FromResult(MessageListOfSongs);
        }
    }
}
