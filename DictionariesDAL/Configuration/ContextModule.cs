using Autofac;
using DictionariesDAL.Interfaces;

namespace DictionariesDAL.Configuration
{
    public class ContextModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DictionaryRepository>().As<IRepository>();
        }
    }
}
