using DictionariesModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace DictionariesDAL.Configuration
{
    public class ItemConfiguration : EntityTypeConfiguration<Item>
    {
        public ItemConfiguration()
        {
            ToTable("Items");
            HasKey(t => t.ItemId); 
            
            Property(t => t.ValueId).HasMaxLength(250);

            Property(t => t.DictionaryId).HasColumnAnnotation(IndexAnnotation.AnnotationName,
                new IndexAnnotation
                    (
                    new IndexAttribute("NIX_Item", 1) {IsUnique = true}
                    )
                );

            Property(t => t.ValueId).HasColumnAnnotation(IndexAnnotation.AnnotationName,
                new IndexAnnotation
                    (
                    new IndexAttribute("NIX_Item", 2) {IsUnique = true}
                    )
                );
        }
    }
}
