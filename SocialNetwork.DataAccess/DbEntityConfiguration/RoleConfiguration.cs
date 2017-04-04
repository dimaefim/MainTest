using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using SocialNetwork.DataAccess.DbEntity;

namespace SocialNetwork.DataAccess.DbEntityConfiguration
{
    public class RoleConfiguration : EntityTypeConfiguration<RoleEntity>
    {
        public RoleConfiguration()
        {
            ToTable("Roles");
            HasKey(t => t.Id);
            Property(t => t.RoleName).IsRequired().HasMaxLength(50).
                HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_RoleNameUnique") { IsUnique = true }));
            HasMany(t => t.UserRoles).WithRequired(t => t.Role).HasForeignKey(t => t.RoleId).WillCascadeOnDelete(false);
        }
    }
}