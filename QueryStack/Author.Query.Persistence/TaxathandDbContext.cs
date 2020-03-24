using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Cosmos.Metadata.Internal;
using System.Linq;
using System.Reflection;
using Author.Query.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using Author.Query.Domain.DBAggregate;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Author.Query.Persistence
{
    public class TaxathandDbContext : DbContext
    {
        public TaxathandDbContext(DbContextOptions<TaxathandDbContext> options) : base(options)
        {
           // Database.EnsureCreated();
        }

        protected TaxathandDbContext()
        {
             //Database.EnsureCreated();
        }

        public DbSet<Address> Address { get; set; }
        public DbSet<Languages> Languages { get; set; }
        public DbSet<Contacts> Contacts { get; set; }
        public DbSet<Countries> Countries { get; set; }
        public DbSet<Disclaimers> Disclaimers { get; set; }
        public DbSet<Provinces> Provinces { get; set; }
        public DbSet<ResourceGroups> ResourceGroups { get; set; }
        public DbSet<SystemUsers> SystemUsers { get; set; }
        public DbSet<UserActivities> UserActivities { get; set; }
        public DbSet<WebsiteUsers> WebsiteUsers { get; set; }
        public DbSet<CountryGroups> CountryGroups { get; set; }
        public DbSet<TaxTags> TaxTags { get; set; }
        public DbSet<Articles> Articles { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<SearchSource> SearchSource { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new NumberToStringConverter<int>();
            //Assign articles partition
            modelBuilder.Entity<Articles>()
                  .Property(e => e.LanguageId)
                  .HasConversion(converter);
            modelBuilder.Entity<Articles>()
                  .HasPartitionKey(o => o.LanguageId);

            //Assign countrygroups partition
            modelBuilder.Entity<CountryGroups>()
                  .Property(e => e.LanguageId)
                  .HasConversion(converter);
            modelBuilder.Entity<CountryGroups>()
                  .HasPartitionKey(o => o.LanguageId);

            //Assign country partition
            modelBuilder.Entity<Countries>()
                  .Property(e => e.LanguageId)
                  .HasConversion(converter);
            modelBuilder.Entity<Countries>()
                  .HasPartitionKey(o => o.LanguageId);

            //Assign address partition
            modelBuilder.Entity<Address>()
                  .HasPartitionKey(o => o.Country);

            //Assign languages partition
            modelBuilder.Entity<Languages>()
                  .HasPartitionKey(o => o.Name);

            //Assign contacts partition
            modelBuilder.Entity<Contacts>()
                  .Property(e => e.CountryId)
                  .HasConversion(converter);
            modelBuilder.Entity<Contacts>()
                  .HasPartitionKey(o => o.CountryId);

            //Assign disclaimers partition
            modelBuilder.Entity<Disclaimers>()
                  .Property(e => e.LanguageId)
                  .HasConversion(converter);
            modelBuilder.Entity<Disclaimers>()
                  .HasPartitionKey(o => o.LanguageId);

            //Assign provinces partition
            modelBuilder.Entity<Provinces>()
                  .Property(e => e.CountryId)
                  .HasConversion(converter);
            modelBuilder.Entity<Provinces>()
                  .HasPartitionKey(o => o.CountryId);

            //Assign resourcegroups partition
            modelBuilder.Entity<ResourceGroups>()
                  .Property(e => e.LanguageId)
                  .HasConversion(converter);
            modelBuilder.Entity<ResourceGroups>()
                  .HasPartitionKey(o => o.LanguageId);

            //Assign systemusers partition
            modelBuilder.Entity<SystemUsers>()
                  .Property(e => e.CountryId)
                  .HasConversion(converter);
            modelBuilder.Entity<SystemUsers>()
                  .HasPartitionKey(o => o.CountryId);

            //Assign websiteusers partition
            modelBuilder.Entity<WebsiteUsers>()
                  .HasPartitionKey(o => o.IndustryName);

            //Assign taxtags partition
            modelBuilder.Entity<TaxTags>()
                  .Property(e => e.LanguageId)
                  .HasConversion(converter);
            modelBuilder.Entity<TaxTags>()
                  .HasPartitionKey(o => o.LanguageId);

            //Assign images partition
            modelBuilder.Entity<Images>()
                  .Property(e => e.CountryId)
                  .HasConversion(converter);
            modelBuilder.Entity<Images>()
                  .HasPartitionKey(o => o.CountryId);

            //Assign useractivities partition
            modelBuilder.Entity<UserActivities>()
                  .Property(e => e.WebsiteUserId)
                  .HasConversion(converter);
            modelBuilder.Entity<UserActivities>()
                  .HasPartitionKey(o => o.WebsiteUserId);

            modelBuilder.Entity<CountryGroups>().OwnsMany(p => p.AssociatedCountryIds, a =>
            {
                a.WithOwner().HasForeignKey("CountryGroupsid");
                a.Property<int>("id");
                a.Property(o => o.IdVal);
            });

            // For TaxTags
            modelBuilder.Entity<TaxTags>().OwnsMany(p => p.RelatedCountryIds, a =>
            {
                a.WithOwner().HasForeignKey("TaxTagid");
                a.Property<int>("id");
                a.Property(o => o.IdVal);
            });

            // For Articles
            ////modelBuilder.Entity<Articles>().OwnsMany(p => p.RelatedCountries, a =>
            ////{
            ////    a.WithOwner().HasForeignKey("Articlesid");
            ////    a.Property<int>("id");
            ////    a.Property(o => o.IdVal);
            ////}).OwnsMany(p => p.RelatedContacts, a =>
            ////{
            ////    a.WithOwner().HasForeignKey("Articlesid");
            ////    a.Property<int>("id");
            ////    a.Property(o => o.IdVal);
            ////}).OwnsMany(p => p.RelatedCountryGroups, a =>
            ////{
            ////    a.WithOwner().HasForeignKey("Articlesid");
            ////    a.Property<int>("id");
            ////    a.Property(o => o.IdVal);
            ////}).OwnsMany(p => p.RelatedTaxTags, a =>
            ////{
            ////    a.WithOwner().HasForeignKey("Articlesid");
            ////    a.Property<int>("id");
            ////    a.Property(o => o.TaxTagId);
            ////    a.Property(o => o.DisplayName);
            //////}).OwnsMany(p => p.RelatedArticles, a =>
            //////{
            //////    a.WithOwner().HasForeignKey("Articlesid");
            //////    a.Property<int>("id");
            //////    a.Property(o => o.ArticleId);
            //////    a.Property(o => o.Title);
            //////    a.Property(o => o.PublishedDate);
            //////    a.OwnsMany(p => p.RelatedCountries, a =>
            //////    {
            //////        a.WithOwner().HasForeignKey("RelatedArticlesSchemaArticlesidid");
            //////        a.Property<int>("id");
            //////        a.Property(o => o.IdVal);
            //////    });
            //////}).OwnsMany(p => p.RelatedResources, a =>
            //////{
            //////    a.OwnsMany(p => p.RelatedCountries, a =>
            //////    {
            //////        a.WithOwner().HasForeignKey("RelatedArticlesSchemaArticlesidid");
            //////        a.Property<int>("id");
            //////        a.Property(o => o.IdVal);
            //////    });
            //////    a.WithOwner().HasForeignKey("Articlesid");
            //////    a.Property<int>("id");
            //////    a.Property(o => o.ArticleId);
            //////    a.Property(o => o.Title);
            //////    //a.Property(o => o.RelatedCountries);
            //////    a.Property(o => o.PublishedDate);
            ////}).OwnsOne(p => p.Provisions, a =>
            ////{
            ////    a.WithOwner().HasForeignKey("Articlesid");
            ////    a.Property<int>("id");
            ////    a.Property(o => o.ProvinceId);
            ////    a.Property(o => o.DisplayName);
            ////}).OwnsOne(p => p.ResourceGroup, a =>
            ////{
            ////    a.WithOwner().HasForeignKey("Articlesid");
            ////    a.Property<int>("id");
            ////    a.Property(o => o.ResourceGroupId);
            ////    a.Property(o => o.Position);
            ////    a.Property(o => o.GroupName);
            ////}).OwnsOne(p => p.Disclaimer, a =>
            ////{
            ////    a.WithOwner().HasForeignKey("Articlesid");
            ////    a.Property<int>("id");
            ////    a.Property(o => o.DisclaimerId);
            ////    a.Property(o => o.ProviderName);
            ////    a.Property(o => o.ProviderTerms);
            ////});

            modelBuilder.Entity<Articles>().OwnsMany(p => p.RelatedCountries);
            modelBuilder.Entity<Articles>().OwnsMany(p => p.RelatedContacts);
            modelBuilder.Entity<Articles>().OwnsMany(p => p.RelatedCountryGroups);
            modelBuilder.Entity<Articles>().OwnsMany(p => p.RelatedTaxTags);
            modelBuilder.Entity<Articles>().OwnsMany(p => p.RelatedArticles, a => { a.ToJsonProperty("RelatedArticles"); a.OwnsMany(p => p.RelatedCountries); });
            modelBuilder.Entity<Articles>().OwnsMany(p => p.RelatedResources, a => { a.ToJsonProperty("RelatedResources"); a.OwnsMany(p => p.RelatedCountries); });
            modelBuilder.Entity<Articles>().OwnsOne(p => p.Province);
            modelBuilder.Entity<Articles>().OwnsOne(p => p.ResourceGroup);
            modelBuilder.Entity<Articles>().OwnsOne(p => p.Disclaimer);
            modelBuilder.Entity<UserActivities>().OwnsMany(p => p.SavedArticles);
            modelBuilder.Entity<UserActivities>().OwnsMany(p => p.ReadArticles);
            modelBuilder.Entity<UserActivities>().OwnsMany(p => p.SubscribedCountries, a => { a.ToJsonProperty("SubscribedCountries"); a.OwnsMany(p => p.Country); });

            OneCollectionPerDbSet(modelBuilder);
        }

        private void OneCollectionPerDbSet(ModelBuilder modelBuilder)
        {
            var dbSets = typeof(TaxathandDbContext).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsGenericType && typeof(DbSet<>).IsAssignableFrom(p.PropertyType.GetGenericTypeDefinition()));
            foreach (var dbSet in dbSets)
            {
                var metadata = modelBuilder.Entity(dbSet.PropertyType.GetGenericArguments()[0]).Metadata;
                //metadata.Cosmos().ContainerName = dbSet.Name;
                metadata.SetContainer(dbSet.Name);
            }
        }
    }

    //public class Languages
    //{
    //    /// <summary>gets or sets the id </summary>
    //    ///// <value>It is of type integer </value>
    //    //[JsonProperty("id")]
    //    public string id { get; set; }
    //    /// <summary>gets or sets the LanguageId </summary>
    //    ///// <value>It is of type integer </value>
    //    //[JsonProperty("LanguageId")]        
    //    public int LanguageId { get; set; }

    //    /// <summary>gets or sets the Name </summary>
    //    /// <value>It is of type string </value>
    //    //[JsonProperty("Name")]
    //    public string Name { get; set; }
    //    /// <summary>gets or sets the NameinEnglish </summary>
    //    /// <value>It is of type string </value>
    //    //[JsonProperty("NameinEnglish")]
    //    public string NameinEnglish { get; set; }

    //    /// <summary>gets or sets the LocalisationIdentifier </summary>
    //    /// <value>It is of type string </value>
    //    //[JsonProperty("LocalisationIdentifier")]
    //    public string LocalisationIdentifier { get; set; }

    //    /// <summary>gets or sets Locale </summary>
    //    /// <value>It is of type string </value>
    //    //[JsonProperty("Locale")]
    //    public string Locale { get; set; }

    //    /// <summary>gets or sets the RightToLeft</summary>
    //    /// <value>It is of type bool </value>
    //    //[JsonProperty("RightToLeft")]
    //    public bool RightToLeft { get; set; }
    //}
}
