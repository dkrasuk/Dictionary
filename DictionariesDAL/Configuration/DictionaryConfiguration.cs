using DictionariesModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace DictionariesDAL.Configuration
{
    public class DictionaryConfiguration: EntityTypeConfiguration<Dictionary>
    {
        public DictionaryConfiguration()
        {
            ToTable("Dictionaries");
            
            HasKey(t=>t.Id);

            Property(t => t.Name)
                .HasMaxLength(100)
                .IsRequired()
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation
                        (
                        new IndexAttribute("NIX_Dictionary", 1) {IsUnique = true}
                        )
                );

            Property(t => t.Version)
                .HasMaxLength(100)
                .IsRequired()
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation
                        (
                        new IndexAttribute("NIX_Dictionary", 2) {IsUnique = true}
                        )
                );

            Property(t => t.Metadata)
                .HasMaxLength(2000);
        }
    }
}
