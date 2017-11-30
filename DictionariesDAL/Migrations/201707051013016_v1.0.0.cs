using System.Configuration;
using System.Data.Entity.Migrations;

namespace DictionariesDAL.Migrations
{
    public partial class v100 : DbMigration
    {
        private static string SchemaName => ConfigurationManager.AppSettings["DatabaseSchema"];

        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            CreateTable(
                $"{SchemaName}.Dictionaries",
                c => new
                    {
                        Id = c.Decimal(nullable: false, precision: 10, scale: 0, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Version = c.String(nullable: false, maxLength: 100),
                        Metadata = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.Name, t.Version }, unique: true, name: "NIX_Dictionary");
            
            CreateTable(
                $"{SchemaName}.Items",
                c => new
                    {
                        ItemId = c.Decimal(nullable: false, precision: 10, scale: 0, identity: true),
                        DictionaryId = c.Decimal(nullable: false, precision: 10, scale: 0),
                        ValueString = c.String(),
                        ValueId = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.ItemId)
                .ForeignKey($"{SchemaName}.Dictionaries", t => t.DictionaryId, cascadeDelete: true)
                .Index(t => new { t.DictionaryId, t.ValueId }, unique: true, name: "NIX_Item");
            
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
            DropForeignKey($"{SchemaName}.Items", "DictionaryId", $"{SchemaName}.Dictionaries");
            DropIndex($"{SchemaName}.Items", "NIX_Item");
            DropIndex($"{SchemaName}.Dictionaries", "NIX_Dictionary");
            DropTable($"{SchemaName}.Items");
            DropTable($"{SchemaName}.Dictionaries");
        }
    }
}
