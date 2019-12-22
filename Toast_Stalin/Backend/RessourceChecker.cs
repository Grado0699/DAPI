using System.IO;

namespace Toast_Stalin.Backend
{
    public static class RessourceChecker
    {
        public static bool IsRessourceAvailable(string RessourceName)
        {
            if (File.Exists(RessourceName))
            {
                return true;
            }

            return false;
        }
    }
}
