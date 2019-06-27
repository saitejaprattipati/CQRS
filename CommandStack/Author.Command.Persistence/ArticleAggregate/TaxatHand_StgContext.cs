using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Design;
using System.Data;
namespace Author.Command.Persistence.Author.Command.API.ArticleAggregate
{
    public partial class TaxatHand_StgContext : DbContext, IUnitOfWork
    {
        public TaxatHand_StgContext()
        {
        }

        public TaxatHand_StgContext(DbContextOptions<TaxatHand_StgContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AddressContents> AddressContents { get; set; }
        public virtual DbSet<Addresses> Addresses { get; set; }
        public virtual DbSet<ArticleContents> ArticleContents { get; set; }
        public virtual DbSet<ArticleRelatedContacts> ArticleRelatedContacts { get; set; }
        public virtual DbSet<ArticleRelatedCountries> ArticleRelatedCountries { get; set; }
        public virtual DbSet<ArticleRelatedCountryGroups> ArticleRelatedCountryGroups { get; set; }
        public virtual DbSet<ArticleRelatedTaxTags> ArticleRelatedTaxTags { get; set; }
        public virtual DbSet<Articles> Articles { get; set; }
        public virtual DbSet<AuthenticationUrlRequests> AuthenticationUrlRequests { get; set; }
        public virtual DbSet<ContactContents> ContactContents { get; set; }
        public virtual DbSet<Contacts> Contacts { get; set; }
        public virtual DbSet<Countries> Countries { get; set; }
        public virtual DbSet<CountryContents> CountryContents { get; set; }
        public virtual DbSet<CountryGroupAssociatedCountries> CountryGroupAssociatedCountries { get; set; }
        public virtual DbSet<CountryGroupContents> CountryGroupContents { get; set; }
        public virtual DbSet<CountryGroups> CountryGroups { get; set; }
        public virtual DbSet<DisclaimerContents> DisclaimerContents { get; set; }
        public virtual DbSet<Disclaimers> Disclaimers { get; set; }
        public virtual DbSet<Images> Images { get; set; }
        public virtual DbSet<Languages> Languages { get; set; }
        public virtual DbSet<Pdicontacts> Pdicontacts { get; set; }
        public virtual DbSet<Pdiimages> Pdiimages { get; set; }
        public virtual DbSet<ProvinceContents> ProvinceContents { get; set; }
        public virtual DbSet<Provinces> Provinces { get; set; }
        public virtual DbSet<RelatedArticles> RelatedArticles { get; set; }
        public virtual DbSet<RelatedResources> RelatedResources { get; set; }
        public virtual DbSet<ResourceGroupContents> ResourceGroupContents { get; set; }
        public virtual DbSet<ResourceGroups> ResourceGroups { get; set; }
        public virtual DbSet<Skills> Skills { get; set; }
        public virtual DbSet<SystemUsers> SystemUsers { get; set; }
        public virtual DbSet<TaxTagContents> TaxTagContents { get; set; }
        public virtual DbSet<TaxTagRelatedCountries> TaxTagRelatedCountries { get; set; }
        public virtual DbSet<TaxTags> TaxTags { get; set; }
        public virtual DbSet<TokenCacheEntries> TokenCacheEntries { get; set; }
        public virtual DbSet<UserDevices> UserDevices { get; set; }
        public virtual DbSet<UserReadArticles> UserReadArticles { get; set; }
        public virtual DbSet<UserSavedArticles> UserSavedArticles { get; set; }
        public virtual DbSet<UserSubscribedCountries> UserSubscribedCountries { get; set; }
        public virtual DbSet<UserSubscribedCountryTags> UserSubscribedCountryTags { get; set; }
        public virtual DbSet<WebsiteUsers> WebsiteUsers { get; set; }

        // Unable to generate entity type for table 'dbo.PDIContacts_Staging'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.Languages_Master'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.PDIImages_Staging'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.PDIImages_Link'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.SystemUserAssociatedCountries'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"Server=taxathand.database.windows.net;Initial Catalog=TaxatHand_AzureDB;Persist Security Info=False;User ID=Taxathand_Admin;Password=tax@hand#1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;", builder =>
                {
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
                base.OnConfiguring(optionsBuilder);
            }
        }
        private readonly IMediator _mediator;
        private IDbContextTransaction _currentTransaction;

      

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;

        public TaxatHand_StgContext(DbContextOptions<TaxatHand_StgContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));


            System.Diagnostics.Debug.WriteLine("TaxatHand_StgContext::ctor ->" + this.GetHashCode());
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AddressContents>(entity =>
            {
                entity.HasKey(e => e.AddressContentId);

                entity.HasIndex(e => e.AddressId)
                    .HasName("IX_AddressId");

                entity.HasIndex(e => e.LanguageId)
                    .HasName("IX_LanguageId");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.AddressContents)
                    .HasForeignKey(d => d.AddressId)
                    .HasConstraintName("FK_dbo.AddressContents_dbo.Addresses_AddressId");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.AddressContents)
                    .HasForeignKey(d => d.LanguageId)
                    .HasConstraintName("FK_dbo.AddressContents_dbo.Languages_LanguageId");
            });

            modelBuilder.Entity<Addresses>(entity =>
            {
                entity.HasKey(e => e.AddressId);
            });

            modelBuilder.Entity<ArticleContents>(entity =>
            {
                entity.HasKey(e => e.ArticleContentId);

                entity.HasIndex(e => e.ArticleId)
                    .HasName("IX_ArticleId");

                entity.HasIndex(e => e.LanguageId)
                    .HasName("IX_LanguageId");

                entity.Property(e => e.Content).IsRequired();

                entity.Property(e => e.TeaserText).IsRequired();

                entity.Property(e => e.Title).IsRequired();

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.ArticleContents)
                    .HasForeignKey(d => d.ArticleId)
                    .HasConstraintName("FK_dbo.ArticleContents_dbo.Articles_ArticleId");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.ArticleContents)
                    .HasForeignKey(d => d.LanguageId)
                    .HasConstraintName("FK_dbo.ArticleContents_dbo.Languages_LanguageId");
            });

            modelBuilder.Entity<ArticleRelatedContacts>(entity =>
            {
                entity.HasKey(e => new { e.ArticleId, e.ContactId });

                entity.HasIndex(e => e.ArticleId)
                    .HasName("IX_ArticleId");

                entity.HasIndex(e => e.ContactId)
                    .HasName("IX_ContactId");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.ArticleRelatedContacts)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ArticleRelatedContacts_dbo.Articles_ArticleId");

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.ArticleRelatedContacts)
                    .HasForeignKey(d => d.ContactId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ArticleRelatedContacts_dbo.Contacts_ContactId");
            });

            modelBuilder.Entity<ArticleRelatedCountries>(entity =>
            {
                entity.HasKey(e => new { e.ArticleId, e.CountryId });

                entity.HasIndex(e => e.ArticleId)
                    .HasName("IX_ArticleId");

                entity.HasIndex(e => e.CountryId)
                    .HasName("IX_CountryId");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.ArticleRelatedCountries)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ArticleRelatedCountries_dbo.Articles_ArticleId");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.ArticleRelatedCountries)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ArticleRelatedCountries_dbo.Countries_CountryId");
            });

            modelBuilder.Entity<ArticleRelatedCountryGroups>(entity =>
            {
                entity.HasKey(e => new { e.ArticleId, e.CountryGroupId });

                entity.HasIndex(e => e.CountryGroupId)
                    .HasName("IX_CountryGroupId");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.ArticleRelatedCountryGroups)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ArticleRelatedCountryGroups_dbo.Articles_ArticleId");

                entity.HasOne(d => d.CountryGroup)
                    .WithMany(p => p.ArticleRelatedCountryGroups)
                    .HasForeignKey(d => d.CountryGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ArticleRelatedCountryGroups_dbo.CountryGroups_CountryGroupId");
            });

            modelBuilder.Entity<ArticleRelatedTaxTags>(entity =>
            {
                entity.HasKey(e => new { e.ArticleId, e.TaxTagId });

                entity.HasIndex(e => e.ArticleId)
                    .HasName("IX_ArticleId");

                entity.HasIndex(e => e.TaxTagId)
                    .HasName("IX_TaxTagId");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.ArticleRelatedTaxTags)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ArticleRelatedTaxTags_dbo.Articles_ArticleId");

                entity.HasOne(d => d.TaxTag)
                    .WithMany(p => p.ArticleRelatedTaxTags)
                    .HasForeignKey(d => d.TaxTagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ArticleRelatedTaxTags_dbo.TaxTags_TaxTagId");
            });

            modelBuilder.Entity<Articles>(entity =>
            {
                entity.HasKey(e => e.ArticleId);

                entity.HasIndex(e => e.DisclaimerId)
                    .HasName("IX_DisclaimerId");

                entity.HasIndex(e => e.ImageId)
                    .HasName("IX_ImageId");

                entity.HasIndex(e => e.ResourceGroupId)
                    .HasName("IX_ResourceGroupId");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.NotificationSentDate).HasColumnType("datetime");

                entity.Property(e => e.PublishedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Disclaimer)
                    .WithMany(p => p.Articles)
                    .HasForeignKey(d => d.DisclaimerId)
                    .HasConstraintName("FK_dbo.Articles_dbo.Disclaimers_DisclaimerId");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.Articles)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_dbo.Articles_dbo.Images_ImageId");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.Articles)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_dbo.Articles_dbo.Provinces_ProvinceId");

                entity.HasOne(d => d.ResourceGroup)
                    .WithMany(p => p.Articles)
                    .HasForeignKey(d => d.ResourceGroupId)
                    .HasConstraintName("FK_dbo.Articles_dbo.ResourceGroups_ResourceGroupId");
            });

            modelBuilder.Entity<AuthenticationUrlRequests>(entity =>
            {
                entity.HasKey(e => e.CookieId);

                entity.Property(e => e.CookieId).ValueGeneratedNever();

                entity.Property(e => e.CookieCreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ContactContents>(entity =>
            {
                entity.HasKey(e => e.ContactContentId);

                entity.HasIndex(e => e.AddressId)
                    .HasName("IX_AddressId");

                entity.HasIndex(e => e.ContactId)
                    .HasName("IX_ContactId");

                entity.HasIndex(e => e.LanguageId)
                    .HasName("IX_LanguageId");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.ContactContents)
                    .HasForeignKey(d => d.AddressId)
                    .HasConstraintName("FK_dbo.ContactContents_dbo.Addresses_AddressId");

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.ContactContents)
                    .HasForeignKey(d => d.ContactId)
                    .HasConstraintName("FK_dbo.ContactContents_dbo.Contacts_ContactId");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.ContactContents)
                    .HasForeignKey(d => d.LanguageId)
                    .HasConstraintName("FK_dbo.ContactContents_dbo.Languages_LanguageId");
            });

            modelBuilder.Entity<Contacts>(entity =>
            {
                entity.HasKey(e => e.ContactId);

                entity.HasIndex(e => e.CountryId)
                    .HasName("IX_CountryId");

                entity.HasIndex(e => e.ImageId)
                    .HasName("IX_ImageId");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmpGuid).HasColumnName("EmpGUID");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Vkurl).HasColumnName("VKUrl");

                entity.Property(e => e.Xingurl).HasColumnName("XINGUrl");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Contacts)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK_dbo.Contacts_dbo.Countries_CountryId");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.Contacts)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_dbo.Contacts_dbo.Images_ImageId");
            });

            modelBuilder.Entity<Countries>(entity =>
            {
                entity.HasKey(e => e.CountryId);

                entity.HasIndex(e => e.PngimageId)
                    .HasName("IX_PNGImageId");

                entity.HasIndex(e => e.SvgimageId)
                    .HasName("IX_SVGImageId");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.PngimageId).HasColumnName("PNGImageId");

                entity.Property(e => e.SvgimageId).HasColumnName("SVGImageId");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Pngimage)
                    .WithMany(p => p.CountriesPngimage)
                    .HasForeignKey(d => d.PngimageId)
                    .HasConstraintName("FK_dbo.Countries_dbo.Images_PNGImageId");

                entity.HasOne(d => d.Svgimage)
                    .WithMany(p => p.CountriesSvgimage)
                    .HasForeignKey(d => d.SvgimageId)
                    .HasConstraintName("FK_dbo.Countries_dbo.Images_SVGImageId");
            });

            modelBuilder.Entity<CountryContents>(entity =>
            {
                entity.HasKey(e => e.CountryContentId);

                entity.HasIndex(e => e.CountryId)
                    .HasName("IX_CountryId");

                entity.HasIndex(e => e.LanguageId)
                    .HasName("IX_LanguageId");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.CountryContents)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK_dbo.CountryContents_dbo.Countries_CountryId");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.CountryContents)
                    .HasForeignKey(d => d.LanguageId)
                    .HasConstraintName("FK_dbo.CountryContents_dbo.Languages_LanguageId");
            });

            modelBuilder.Entity<CountryGroupAssociatedCountries>(entity =>
            {
                entity.HasKey(e => new { e.CountryGroupId, e.CountryId });

                entity.HasIndex(e => e.CountryGroupId)
                    .HasName("IX_CountryGroupId");

                entity.HasIndex(e => e.CountryId)
                    .HasName("IX_CountryId");

                entity.HasOne(d => d.CountryGroup)
                    .WithMany(p => p.CountryGroupAssociatedCountries)
                    .HasForeignKey(d => d.CountryGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.CountryGroupAssociatedCountries_dbo.CountryGroups_CountryGroupId");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.CountryGroupAssociatedCountries)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.CountryGroupAssociatedCountries_dbo.Countries_CountryId");
            });

            modelBuilder.Entity<CountryGroupContents>(entity =>
            {
                entity.HasKey(e => e.CountryGroupContentId);

                entity.HasIndex(e => e.CountryGroupId)
                    .HasName("IX_CountryGroupId");

                entity.HasIndex(e => e.LanguageId)
                    .HasName("IX_LanguageId");

                entity.HasOne(d => d.CountryGroup)
                    .WithMany(p => p.CountryGroupContents)
                    .HasForeignKey(d => d.CountryGroupId)
                    .HasConstraintName("FK_dbo.CountryGroupContents_dbo.CountryGroups_CountryGroupId");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.CountryGroupContents)
                    .HasForeignKey(d => d.LanguageId)
                    .HasConstraintName("FK_dbo.CountryGroupContents_dbo.Languages_LanguageId");
            });

            modelBuilder.Entity<CountryGroups>(entity =>
            {
                entity.HasKey(e => e.CountryGroupId);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<DisclaimerContents>(entity =>
            {
                entity.HasKey(e => e.DisclaimerContentId);

                entity.HasIndex(e => e.DisclaimerId)
                    .HasName("IX_DisclaimerId");

                entity.HasIndex(e => e.LanguageId)
                    .HasName("IX_LanguageId");

                entity.HasOne(d => d.Disclaimer)
                    .WithMany(p => p.DisclaimerContents)
                    .HasForeignKey(d => d.DisclaimerId)
                    .HasConstraintName("FK_dbo.DisclaimerContents_dbo.Disclaimers_DisclaimerId");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.DisclaimerContents)
                    .HasForeignKey(d => d.LanguageId)
                    .HasConstraintName("FK_dbo.DisclaimerContents_dbo.Languages_LanguageId");
            });

            modelBuilder.Entity<Disclaimers>(entity =>
            {
                entity.HasKey(e => e.DisclaimerId);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Images>(entity =>
            {
                entity.HasKey(e => e.ImageId);

                entity.HasIndex(e => e.CountryId)
                    .HasName("IX_CountryId");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmpGuid).HasColumnName("EmpGUID");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Images)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK_dbo.Images_dbo.Countries_CountryId");
            });

            modelBuilder.Entity<Languages>(entity =>
            {
                entity.HasKey(e => e.LanguageId);
            });

            modelBuilder.Entity<Pdicontacts>(entity =>
            {
                entity.HasKey(e => e.PdiContactId);

                entity.ToTable("PDIContacts");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmpGuid)
                    .IsRequired()
                    .HasColumnName("EmpGUID");

                entity.Property(e => e.EmployeeLevel).HasMaxLength(500);

                entity.Property(e => e.PostCode).HasMaxLength(200);

                entity.Property(e => e.Role).HasMaxLength(500);

                entity.Property(e => e.Title).HasMaxLength(500);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Pdiimages>(entity =>
            {
                entity.HasKey(e => e.PdiimageId);

                entity.ToTable("PDIImages");

                entity.Property(e => e.PdiimageId).HasColumnName("PDIImageId");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmpGuid).HasColumnName("EmpGUID");

                entity.Property(e => e.FileType).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProvinceContents>(entity =>
            {
                entity.HasKey(e => e.ProvinceContentId);

                entity.HasIndex(e => e.LanguageId)
                    .HasName("IX_LanguageId");

                entity.HasIndex(e => e.ProvinceId)
                    .HasName("IX_ProvinceId");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.ProvinceContents)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProvinceContents_dbo.Languages_LanguageId");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.ProvinceContents)
                    .HasForeignKey(d => d.ProvinceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.ProvinceContents_dbo.Provinces_ProvinceId");
            });

            modelBuilder.Entity<Provinces>(entity =>
            {
                entity.HasKey(e => e.ProvinceId);

                entity.HasIndex(e => e.CountryId)
                    .HasName("IX_CountryId");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Provinces)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Provinces_dbo.Countries_CountryId");
            });

            modelBuilder.Entity<RelatedArticles>(entity =>
            {
                entity.HasKey(e => new { e.ArticleId, e.RelatedArticleId });

                entity.HasIndex(e => e.ArticleId)
                    .HasName("IX_ArticleId");

                entity.HasIndex(e => e.RelatedArticleId)
                    .HasName("IX_RelatedArticleId");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.RelatedArticlesArticle)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.RelatedArticles_dbo.Articles_ArticleId");

                entity.HasOne(d => d.RelatedArticle)
                    .WithMany(p => p.RelatedArticlesRelatedArticle)
                    .HasForeignKey(d => d.RelatedArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.RelatedArticles_dbo.Articles_RelatedArticleId");
            });

            modelBuilder.Entity<RelatedResources>(entity =>
            {
                entity.HasKey(e => new { e.ArticleId, e.RelatedArticleId });

                entity.HasIndex(e => e.ArticleId)
                    .HasName("IX_ArticleId");

                entity.HasIndex(e => e.RelatedArticleId)
                    .HasName("IX_RelatedArticleId");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.RelatedResourcesArticle)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.RelatedResources_dbo.Articles_ArticleId");

                entity.HasOne(d => d.RelatedArticle)
                    .WithMany(p => p.RelatedResourcesRelatedArticle)
                    .HasForeignKey(d => d.RelatedArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.RelatedResources_dbo.Articles_RelatedArticleId");
            });

            modelBuilder.Entity<ResourceGroupContents>(entity =>
            {
                entity.HasKey(e => e.ResourceGroupContentId);

                entity.HasIndex(e => e.LanguageId)
                    .HasName("IX_LanguageId");

                entity.HasIndex(e => e.ResourceGroupId)
                    .HasName("IX_ResourceGroupId");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.ResourceGroupContents)
                    .HasForeignKey(d => d.LanguageId)
                    .HasConstraintName("FK_dbo.ResourceGroupContents_dbo.Languages_LanguageId");

                entity.HasOne(d => d.ResourceGroup)
                    .WithMany(p => p.ResourceGroupContents)
                    .HasForeignKey(d => d.ResourceGroupId)
                    .HasConstraintName("FK_dbo.ResourceGroupContents_dbo.ResourceGroups_ResourceGroupId");
            });

            modelBuilder.Entity<ResourceGroups>(entity =>
            {
                entity.HasKey(e => e.ResourceGroupId);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Skills>(entity =>
            {
                entity.HasKey(e => e.SkillId);

                entity.HasIndex(e => e.ContactContentId)
                    .HasName("IX_ContactContentId");

                entity.HasOne(d => d.ContactContent)
                    .WithMany(p => p.Skills)
                    .HasForeignKey(d => d.ContactContentId)
                    .HasConstraintName("FK_dbo.Skills_dbo.ContactContents_ContactContentId");
            });

            modelBuilder.Entity<SystemUsers>(entity =>
            {
                entity.HasKey(e => e.SystemUserId);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TaxTagContents>(entity =>
            {
                entity.HasKey(e => e.TaxTagContentId);

                entity.HasIndex(e => e.LanguageId)
                    .HasName("IX_LanguageId");

                entity.HasIndex(e => e.TaxTagId)
                    .HasName("IX_TaxTagId");

                entity.Property(e => e.DisplayName).IsRequired();

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.TaxTagContents)
                    .HasForeignKey(d => d.LanguageId)
                    .HasConstraintName("FK_dbo.TaxTagContents_dbo.Languages_LanguageId");

                entity.HasOne(d => d.TaxTag)
                    .WithMany(p => p.TaxTagContents)
                    .HasForeignKey(d => d.TaxTagId)
                    .HasConstraintName("FK_dbo.TaxTagContents_dbo.TaxTags_TaxTagId");
            });

            modelBuilder.Entity<TaxTagRelatedCountries>(entity =>
            {
                entity.HasKey(e => new { e.TaxTagId, e.CountryId });

                entity.HasIndex(e => e.CountryId)
                    .HasName("IX_CountryId");

                entity.HasIndex(e => e.TaxTagId)
                    .HasName("IX_TaxTagId");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.TaxTagRelatedCountries)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.TaxTagRelatedCountries_dbo.Countries_CountryId");

                entity.HasOne(d => d.TaxTag)
                    .WithMany(p => p.TaxTagRelatedCountries)
                    .HasForeignKey(d => d.TaxTagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.TaxTagRelatedCountries_dbo.TaxTags_TaxTagId");
            });

            modelBuilder.Entity<TaxTags>(entity =>
            {
                entity.HasKey(e => e.TaxTagId);

                entity.HasIndex(e => e.ParentTagId)
                    .HasName("IX_ParentTagId");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.ParentTag)
                    .WithMany(p => p.InverseParentTag)
                    .HasForeignKey(d => d.ParentTagId)
                    .HasConstraintName("FK_dbo.TaxTags_dbo.TaxTags_ParentTagId");
            });

            modelBuilder.Entity<TokenCacheEntries>(entity =>
            {
                entity.HasKey(e => e.TokenCacheEntryId);

                entity.Property(e => e.TokenCacheEntryId).HasColumnName("TokenCacheEntryID");

                entity.Property(e => e.CacheBits).IsRequired();

                entity.Property(e => e.LastWrite).HasColumnType("datetime");

                entity.Property(e => e.UserObjectId)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserDevices>(entity =>
            {
                entity.HasKey(e => e.UserDeviceId);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeviceIdentifier)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.InstallationId)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Platform)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.WebsiteUser)
                    .WithMany(p => p.UserDevices)
                    .HasForeignKey(d => d.WebsiteUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.Devices_dbo.WebsiteUsers_WebsiteUserId");
            });

            modelBuilder.Entity<UserReadArticles>(entity =>
            {
                entity.HasKey(e => e.UserReadArticleId);

                entity.HasIndex(e => e.ArticleId)
                    .HasName("IX_ArticleId");

                entity.HasIndex(e => e.WebsiteUserId)
                    .HasName("IX_WebsiteUserId");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.UserReadArticles)
                    .HasForeignKey(d => d.ArticleId)
                    .HasConstraintName("FK_dbo.UserReadArticles_dbo.Articles_ArticleId");

                entity.HasOne(d => d.WebsiteUser)
                    .WithMany(p => p.UserReadArticles)
                    .HasForeignKey(d => d.WebsiteUserId)
                    .HasConstraintName("FK_dbo.UserReadArticles_dbo.WebsiteUsers_WebsiteUserId");
            });

            modelBuilder.Entity<UserSavedArticles>(entity =>
            {
                entity.HasKey(e => e.UserSavedArticleId);

                entity.HasIndex(e => e.ArticleId)
                    .HasName("IX_ArticleId");

                entity.HasIndex(e => e.WebsiteUserId)
                    .HasName("IX_WebsiteUserId");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.UserSavedArticles)
                    .HasForeignKey(d => d.ArticleId)
                    .HasConstraintName("FK_dbo.UserSavedArticles_dbo.Articles_ArticleId");

                entity.HasOne(d => d.WebsiteUser)
                    .WithMany(p => p.UserSavedArticles)
                    .HasForeignKey(d => d.WebsiteUserId)
                    .HasConstraintName("FK_dbo.UserSavedArticles_dbo.WebsiteUsers_WebsiteUserId");
            });

            modelBuilder.Entity<UserSubscribedCountries>(entity =>
            {
                entity.HasKey(e => e.UserSubscribedCountryId);

                entity.HasIndex(e => e.CountryId)
                    .HasName("IX_CountryId");

                entity.HasIndex(e => e.WebsiteUserId)
                    .HasName("IX_WebsiteUserId");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.UserSubscribedCountries)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK_dbo.UserSubscribedCountries_dbo.Countries_CountryId");

                entity.HasOne(d => d.WebsiteUser)
                    .WithMany(p => p.UserSubscribedCountries)
                    .HasForeignKey(d => d.WebsiteUserId)
                    .HasConstraintName("FK_dbo.UserSubscribedCountries_dbo.WebsiteUsers_WebsiteUserId");
            });

            modelBuilder.Entity<UserSubscribedCountryTags>(entity =>
            {
                entity.HasKey(e => new { e.UserSubscribedCountryId, e.TaxTagId });

                entity.HasIndex(e => e.TaxTagId)
                    .HasName("IX_TaxTagId");

                entity.HasIndex(e => e.UserSubscribedCountryId)
                    .HasName("IX_UserSubscribedCountryId");

                entity.HasOne(d => d.TaxTag)
                    .WithMany(p => p.UserSubscribedCountryTags)
                    .HasForeignKey(d => d.TaxTagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.UserSubscribedCountryTags_dbo.TaxTags_TaxTagId");

                entity.HasOne(d => d.UserSubscribedCountry)
                    .WithMany(p => p.UserSubscribedCountryTags)
                    .HasForeignKey(d => d.UserSubscribedCountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.UserSubscribedCountryTags_dbo.UserSubscribedCountries_UserSubscribedCountryId");
            });

            modelBuilder.Entity<WebsiteUsers>(entity =>
            {
                entity.HasKey(e => e.WebsiteUserId);

                entity.HasIndex(e => e.PreferredLanguageId)
                    .HasName("IX_PreferredLanguageId");

                entity.Property(e => e.AccessTokenExpireTime).HasColumnType("datetime");

                entity.Property(e => e.CookieCreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.PreferredLanguage)
                    .WithMany(p => p.WebsiteUsers)
                    .HasForeignKey(d => d.PreferredLanguageId)
                    .HasConstraintName("FK_dbo.WebsiteUsers_dbo.Languages_PreferredLanguageId");
            });
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await base.SaveChangesAsync();

            return true;
        }

      
    }
}
