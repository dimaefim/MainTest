﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using SocialNetwork.DataAccess.DbEntity;

namespace SocialNetwork.DataAccess.DbEntityConfiguration
{
    public class UserConfiguration : EntityTypeConfiguration<UserEntity>
    {
        public UserConfiguration()
        {
            HasKey(t => t.Id);
            Property(t => t.Email).IsRequired().HasMaxLength(128)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_UserEmailUnique") { IsUnique = true }));
            Property(t => t.Passowrd).IsRequired().HasMaxLength(128);
            Property(t => t.Name).IsRequired().HasMaxLength(50);
            Property(t => t.Surname).IsRequired().HasMaxLength(50);
          
            HasMany(t => t.UserRoles).WithRequired(t => t.User).HasForeignKey(t => t.UserId).WillCascadeOnDelete(false);
        }
    }
}