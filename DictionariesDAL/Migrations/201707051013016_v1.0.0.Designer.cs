using System.CodeDom.Compiler;
using System.Data.Entity.Migrations.Infrastructure;
using System.Resources;

namespace DictionariesDAL.Migrations
{
    [GeneratedCode("EntityFramework.Migrations", "6.1.3-40302")]
    public sealed partial class v100 : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(v100));
        
        string IMigrationMetadata.Id
        {
            get { return "201707051013016_v1.0.0"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}
