using System.Text;

namespace AODb.Common
{
    public static class GameText
    {
        // Game strings are Windows-1252; .NET's Encoding.Default is UTF-8.
        public static readonly Encoding Cp1252;

        static GameText()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Cp1252 = Encoding.GetEncoding(1252);
        }
    }
}
