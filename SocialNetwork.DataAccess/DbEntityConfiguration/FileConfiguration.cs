﻿using System.Data.Entity.ModelConfiguration;
using SocialNetwork.DataAccess.DbEntity;

namespace SocialNetwork.DataAccess.DbEntityConfiguration
{
    class FileConfiguration : EntityTypeConfiguration<FileEntity>
    {
        public FileConfiguration()
        {
            ToTable("Files");

            HasKey(t => t.Id);

            Property(p => p.UserSettingsId).IsRequired();
            Property(p => p.Content).IsRequired();
            Property(p => p.DateCreated).IsRequired();
            Property(p => p.MimeType).IsRequired();
            Property(p => p.Name).IsRequired();
        }
    }
}