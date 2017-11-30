using Autofac;
using DictionariesDAL;
using DictionariesDAL.Interfaces;

namespace DictionariesBL.Confguration
{
    public class BLModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DictionaryRepository>().As<IRepository>();
        }
    }
}
