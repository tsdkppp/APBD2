using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("LegacyApp.Tests")]

namespace LegacyApp
{
    public class Client
    {
        public string Name { get; internal set; }
        public int ClientId { get; internal set; }
        public string Email { get; internal set; }
        public string Address { get; internal set; }
        public string Type { get; set; }
    }
}