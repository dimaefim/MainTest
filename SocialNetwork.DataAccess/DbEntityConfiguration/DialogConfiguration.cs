using System.Data.Entity.ModelConfiguration;
using SocialNetwork.DataAccess.DbEntity;

namespace SocialNetwork.DataAccess.DbEntityConfiguration
{
    public class DialogConfiguration : EntityTypeConfiguration<DialogEntity>
    {
        public DialogConfiguration()
        {
            ToTable("Dialogs");

            HasKey(p => p.Id);

            Property(p => p.LastMessageTime).IsRequired();

            HasMany(p => p.Messages).WithRequired(p => p.Diaolg).HasForeignKey(p => p.DialogId).WillCascadeOnDelete(false);
            HasMany(t => t.DialogUsers).WithRequired(t => t.Dialog).HasForeignKey(t => t.DialogId).WillCascadeOnDelete(false);
        }
    }
}