using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Cosmos.Metadata.Internal;
using System.Linq;
using System.Reflection;
using Author.Query.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using Author.Query.Domain.DBAggregate;

namespace Author.Query.Persistence
{
   public class TaxathandDbContext : DbContext
    {
        public TaxathandDbContext(DbContextOptions<TaxathandDbContext> options) : base(options)
        {
            //Database.EnsureCreated();
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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
                {                    
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
