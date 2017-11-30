using Autofac;
using DictionariesBL.Confguration;
using DictionaryApi.Controllers;
using Autofac.Integration.WebApi;
using DictionariesDAL.Interfaces;
using DictionariesDAL;
using System.Web.Http;
using AlfaBank.Logger;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using DictionariesBL;
using DictionaryApi.Services;
using DictionaryApi.Interfaces;

namespace DictionaryApi.Configuration
{
    /// <summary>
    /// Configurate Autofac
    /// </summary>
    public class AutofacConfig:Autofac.Module
    {
        internal static void AutofacRegister()
        {
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(typeof(DictionaryController).Assembly);
            builder.RegisterModule<BLModule>();
            builder.RegisterType<LogTruncator>().As<ILogTruncator>();
            builder.RegisterType<Log4NetLogger>().UsingConstructor(typeof(ILogTruncator));
            builder.RegisterType<Log4NetLogger>().As<ILogger>();
            builder.RegisterType<DictionaryRepository>().As<IRepository>();
            builder.RegisterType<BLService>().As<IBLService>();
            builder.RegisterType<AccountService>().As<IAccountService>();
            builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);
            
            var container = builder.Build();

            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(container));

            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}