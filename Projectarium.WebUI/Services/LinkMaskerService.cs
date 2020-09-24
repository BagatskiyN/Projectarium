using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projectarium.WebUI.Services
{
    ///<summary>
    /// Интерфейс сервиса для создания маски ссылки
    ///</summary>
    public interface ILinkMasker
    {
        public string MaskLink(Uri IncomingLink);

    }
    ///<summary>
    /// Класс сервиса для создания маски ссылки. 
    ///</summary>
    public class LinkMaskerService : ILinkMasker
    {
        ///<summary>
        /// Метод для создания маски ссылки. Маска выводится вместо польного имени ссылки  
        ///</summary>
        ///<params name="IncomingLink">Ссылка</params>
        public string MaskLink(Uri IncomingLink)
        {

            string MaskedForLink = IncomingLink.Host;
            MaskedForLink = GetMask(MaskedForLink);

            return MaskedForLink;

        }
        ///<summary>
        /// Метод достает из ссылки основной текст.Например из https://uk-ua.facebook.com/ получается facebook 
        ///</summary>
        ///<params name="url">Текст ссылки</params>
        private string GetMask(string url)
        {
            string[] split = url.Split('.');
            if (split.Length > 2)
                return split[split.Length - 2];
            else
                return url;

        }


    }
    ///<summary>
    /// Метод расширения для сервиса LinkMaskerService 
    ///</summary>
    public static class LinkMaskerServiceExtention
    {
        public static IServiceCollection LinkMaskerService(this IServiceCollection services)
            => services.AddTransient<ILinkMasker, LinkMaskerService>();
    }
}
