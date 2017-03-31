using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using SocialNetwork.DataAccess.DbEntity;

namespace SocialNetwork.DataAccess.DbEntityConfiguration
{
    class UserSettingsConfiguration : EntityTypeConfiguration<UserSettingsEntity>
    {
        public UserSettingsConfiguration()
        {
            ToTable("UsersSettings");

            HasKey(p => p.Id);

            Property(p => p.AboutMe).IsOptional().HasMaxLength(8000);

            HasRequired(p => p.User).WithOptional(p => p.Settings).WillCascadeOnDelete(false);
            HasMany(p => p.Files).WithRequired(p => p.UserSettings).HasForeignKey(p => p.UserSettingsId).WillCascadeOnDelete(false);
        }
    }
}
