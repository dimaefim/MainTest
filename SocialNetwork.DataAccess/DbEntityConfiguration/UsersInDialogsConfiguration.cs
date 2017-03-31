using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialNetwork.DataAccess.DbEntity;

namespace SocialNetwork.DataAccess.DbEntityConfiguration
{
    public class UsersInDialogsConfiguration : EntityTypeConfiguration<UsersInDialogsEntity>
    {
        public UsersInDialogsConfiguration()
        {
            ToTable("UsersInDialogs");
            HasKey(x => new { x.UserId, x.DialogId });
        }
    }
}
