using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projectarium.WebUI.Services
{
    public interface ILinkMasker
    {
        public string MaskLink(Uri IncomingLink);

    }

    public class LinkMaskerService : ILinkMasker
    {

        public string MaskLink(Uri IncomingLink)
        {

            string MaskedForLink = IncomingLink.Host;
            MaskedForLink = GetMask(MaskedForLink);

            return MaskedForLink;

        }
        private string GetMask(string url)
        {
            string[] split = url.Split('.');
            if (split.Length > 2)
                return split[split.Length - 2];
            else
                return url;

        }


    }
    public static class LinkMaskerServiceExtention
    {
        public static IServiceCollection GetGroupList(this IServiceCollection services)
            => services.AddTransient<ILinkMasker, LinkMaskerService>();
    }
}
