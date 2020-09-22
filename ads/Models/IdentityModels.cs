using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections;

namespace ads.Models
{
    // Чтобы добавить данные профиля для пользователя, можно добавить дополнительные свойства в класс ApplicationUser. Дополнительные сведения см. по адресу: http://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ad> Ads { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Message> MessagesSender { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChatUser> ChatUsers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Review> ReviewsSender { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Review> ReviewsRecipient { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя

            Ads = new HashSet<Ad>();
            MessagesSender = new HashSet<Message>();

            ChatUsers = new HashSet<ChatUser>();

            ReviewsSender = new HashSet<Review>();
            ReviewsRecipient = new HashSet<Review>();

            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {

        }

        public virtual DbSet<Ad> Ads { get; set; }
        public virtual DbSet<AdStatus> AdStatuses { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<ChatUser> ChatUser { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public IEnumerable ApplicationUsers { get; internal set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ad>()
                .Property(e => e.title)
                .IsUnicode(false);

            modelBuilder.Entity<Ad>()
                .Property(e => e.text)
                .IsUnicode(false);
            
            modelBuilder.Entity<Ad>()
                .HasMany(e => e.Chats)
                .WithOptional(e => e.Ad)
                .HasForeignKey(e => e.idAd);

            modelBuilder.Entity<Ad>()
                .HasMany(e => e.Images)
                .WithRequired(e => e.Ad)
                .HasForeignKey(e => e.idAd)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Ad>()
               .Property(e => e.idUser)
               .IsUnicode(false);

            modelBuilder.Entity<Image>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Image>()
                .HasMany(e => e.Ads)
                .WithOptional(e => e.Image)
                .HasForeignKey(e => e.idImage)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AdStatus>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<AdStatus>()
                .HasMany(e => e.Ads)
                .WithOptional(e => e.AdStatus)
                .HasForeignKey(e => e.idStatusAd)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<City>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<City>()
                .HasMany(e => e.Ads)
                .WithOptional(e => e.City)
                .HasForeignKey(e => e.idCity)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Message>()
                .Property(e => e.text)
                .IsUnicode(false);

            modelBuilder.Entity<Message>()
               .Property(e => e.idSender)
               .IsUnicode(false);

            modelBuilder.Entity<Chat>()
               .Property(e => e.title)
               .IsUnicode(false);

            modelBuilder.Entity<Chat>()
            .HasMany(e => e.Messages)
            .WithRequired(e => e.Chat)
            .HasForeignKey(e => e.idChat)
            .WillCascadeOnDelete();

            modelBuilder.Entity<Chat>()
           .HasMany(e => e.ChatUsers)
           .WithRequired(e => e.Chat)
           .HasForeignKey(e => e.idChat)
           .WillCascadeOnDelete(false);

            modelBuilder.Entity<Review>()
                .Property(e => e.text)
                .IsUnicode(false);

            modelBuilder.Entity<Review>()
               .Property(e => e.idSender)
               .IsUnicode(false);

            modelBuilder.Entity<Review>()
               .Property(e => e.idRecipient)
               .IsUnicode(false);

            modelBuilder.Entity<Subject>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<Subject>()
                .HasMany(e => e.Ads)
                .WithOptional(e => e.Subject)
                .HasForeignKey(e => e.idSubject)
                .WillCascadeOnDelete();

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.Ads)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.idUser)
                .WillCascadeOnDelete();

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.MessagesSender)
                .WithRequired(e => e.UserSender)
                .HasForeignKey(e => e.idSender)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.ChatUsers)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.idUser)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.ReviewsSender)
                .WithRequired(e => e.UserSender)
                .HasForeignKey(e => e.idSender)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.ReviewsRecipient)
                .WithRequired(e => e.UserRecipient)
                .HasForeignKey(e => e.idRecipient)
                .WillCascadeOnDelete(false);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}