using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialNetwork.DataAccess.DbEntity;

namespace SocialNetwork.DataAccess.DbEntityConfiguration
{
    public class MessageConfiguration : EntityTypeConfiguration<MessageEntity>
    {
        public MessageConfiguration()
        {
            ToTable("Messages");

            HasKey(t => t.Id);

            Property(p => p.DialogId).IsRequired();
            Property(p => p.Text).IsRequired().HasMaxLength(8000);
            Property(p => p.TimeOfSend).IsRequired();
        }
    }
}
