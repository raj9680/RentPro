using Microsoft.AspNetCore.Mvc;

namespace Rent.UI.Helpers
{
    public static class UrlHelper
    {
        private const string SpritePath = "~/assets/icons/icons.svg";

        public static string Icon(this IUrlHelper url, string name)
            => url.Content(SpritePath) + "#" + name;
    }
}
