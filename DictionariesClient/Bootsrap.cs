using Microsoft.Rest;
using System;
using System.Configuration;
using Unity;
using Unity.Injection;

namespace DictionariesClient
{
    public class Bootsrap
    {
        public static void Register(IUnityContainer container)
        {
            var credentials = new BasicAuthenticationCredentials();
            credentials.UserName = ConfigurationManager.AppSettings["DictionariesApiLogin"];
            credentials.Password = ConfigurationManager.AppSettings["DictionariesApiPassword"];
            var dictionariesClient = new Dictionaries.DictionariesClient(new Uri(ConfigurationManager.AppSettings["DictionariesApiEndpoint"]), credentials);
            container
                .RegisterType<Dictionaries.IDictionaryOperations, Dictionaries.DictionaryOperations>(new InjectionConstructor(dictionariesClient));
        }
    }
}
