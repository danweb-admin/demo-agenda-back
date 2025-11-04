using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Mappings;
using NetDevPack.Data;
using Solucao.Application.Contracts.Response;

namespace Solucao.Application.Data
{
    public class SolucaoContext : DbContext, IUnitOfWork, ISolucaoContext
    {
        public SolucaoContext()
        {
                
        }

        public SolucaoContext(DbContextOptions<SolucaoContext> options)
        : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
            ChangeTracker.LazyLoadingEnabled = true;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Specification> Specifications { get; set; }
        public DbSet<Equipament> Equipaments { get; set; }
        public DbSet<EquipamentSpecifications> EquipamentSpecifications { get; set; }
        public DbSet<Calendar> Calendars { get; set; }
        public DbSet<CalendarSpecifications> CalendarSpecifications { get; set; }
        public DbSet<StickyNote> StickyNotes { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<ModelAttributes> ModelAttributes { get; set; }
        public DbSet<AttributeTypes> AttributeTypes { get; set; }
        public DbSet<TechnicalAttributes> TechnicalAttributes { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<Consumable> Consumables { get; set; }
        public DbSet<EquipamentConsumable> EquipamentConsumables { get; set; }
        public DbSet<CalendarEquipamentConsumable> CalendarEquipamentConsumables { get; set; }
        public DbSet<CalendarSpecificationConsumables> CalendarSpecificationConsumables { get; set; }
        public DbSet<EquipmentRelationship> EquipmentRelantionships { get; set; }
        public DbSet<EquipmentRelationshipEquipment> EquipmentRelationshipEquipment { get; set; }
        public DbSet<ClientEquipment> ClientEquipment { get; set; }
        public DbSet<TimeValue> TimeValues { get; set; }
        public DbSet<ClientSpecification> ClientSpecifications { get; set; }
        public DbSet<DigitalSignature> DigitalSignatures { get; set; }
        public DbSet<DigitalSignatureEvents> DigitalSignatureEvents { get; set; }
        public DbSet<ClientDigitalSignature> ClientDigitalSignatures { get; set; }
        public DbSet<PriceTable> PriceTable { get; set; }


        public DbSet<CalendarReportResponse> CalendarReports { get; set; }
        public DbSet<CalendarViewResponse> CalendarViewResponses { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<ValidationResult>();

            modelBuilder.ApplyConfiguration(new PersonMapping());
            modelBuilder.ApplyConfiguration(new UserMapping());
            modelBuilder.ApplyConfiguration(new ClientMapping());
            modelBuilder.ApplyConfiguration(new StateMapping());
            modelBuilder.ApplyConfiguration(new CityMapping());
            modelBuilder.ApplyConfiguration(new SpecificationMapping());
            modelBuilder.ApplyConfiguration(new EquipamentMapping());
            modelBuilder.ApplyConfiguration(new CalendarMapping());
            modelBuilder.ApplyConfiguration(new CalendarSpecificationsMapping());
            modelBuilder.ApplyConfiguration(new StickyNoteMapping());
            modelBuilder.ApplyConfiguration(new ModelMapping());
            modelBuilder.ApplyConfiguration(new ModelAttribrutesMapping());
            modelBuilder.ApplyConfiguration(new AttributeTypesMapping());
            modelBuilder.ApplyConfiguration(new TechnicalAttributesMapping());
            modelBuilder.ApplyConfiguration(new HistoryMapping());
            modelBuilder.ApplyConfiguration(new ConsumableMapping());
            modelBuilder.ApplyConfiguration(new EquipamentConsumableMapping());
            modelBuilder.ApplyConfiguration(new CalendarEquipamentConsumableMapping());
            modelBuilder.ApplyConfiguration(new CalendarSpecificationConsumableMapping());
            modelBuilder.ApplyConfiguration(new EquipmentRelantionshipMapping());
            modelBuilder.ApplyConfiguration(new EquipmentRelationshipEquipmentMapping());
            modelBuilder.ApplyConfiguration(new TimeValuesMapping());
            modelBuilder.ApplyConfiguration(new ClientEquipmentMapping());
            modelBuilder.ApplyConfiguration(new ClientSpecificationMapping());
            modelBuilder.ApplyConfiguration(new DigitalSignatureMapping());
            modelBuilder.ApplyConfiguration(new DigitalSignatureEventsMapping());
            modelBuilder.ApplyConfiguration(new ClientDigitalSignatureMapping());
            modelBuilder.ApplyConfiguration(new PriceTableMapping());



            // Relationship
            modelBuilder.Entity<State>()
                .HasMany(c => c.Cities)
                .WithOne(e => e.State);

            modelBuilder.Entity<City>()
                .HasOne(e => e.State)
                .WithMany(c => c.Cities);

            modelBuilder.Entity<Client>()
                .HasOne(e => e.State);

            modelBuilder.Entity<StickyNote>()
                .HasOne(e => e.User);

            modelBuilder.Entity<Client>()
                .HasOne(e => e.City);

            modelBuilder.Entity<Calendar>()
                .HasOne(e => e.Client);

            modelBuilder.Entity<Calendar>()
                .HasOne(e => e.Driver);

            modelBuilder.Entity<Calendar>()
                .HasOne(e => e.Technique);

            modelBuilder.Entity<Calendar>()
                .HasOne(e => e.Equipament);

            modelBuilder.Entity<Calendar>()
                .HasOne(e => e.User);

            modelBuilder.Entity<CalendarEquipamentConsumable>()
                .HasOne(e => e.Calendar);

            modelBuilder.Entity<CalendarEquipamentConsumable>()
                .HasOne(e => e.Equipament);

            modelBuilder.Entity<CalendarEquipamentConsumable>()
                .HasOne(e => e.Consumable);

            modelBuilder.Entity<CalendarSpecificationConsumables>()
                .HasOne(e => e.Calendar);

            modelBuilder.Entity<CalendarSpecificationConsumables>()
                .HasOne(e => e.Specification);

            modelBuilder.Entity<Specification>()
               .HasMany(c => c.CalendarSpecifications)
               .WithOne(e => e.Specification);

            modelBuilder.Entity<Client>()
               .HasMany(c => c.ClientEquipment)
               .WithOne(e => e.Client);

            modelBuilder.Entity<Client>()
               .HasMany(c => c.ClientDigitalSignatures)
               .WithOne(e => e.Client);

            modelBuilder.Entity<Specification>()
                .HasMany(c => c.EquipamentSpecifications)
                .WithOne(e => e.Specification);

            modelBuilder.Entity<Consumable>()
                .HasMany(c => c.EquipamentConsumables)
                .WithOne(e => e.Consumable);

            modelBuilder.Entity<Equipament>()
                .HasMany(c => c.EquipamentSpecifications)
                .WithOne(e => e.Equipament);

            modelBuilder.Entity<Equipament>()
                .HasMany(c => c.EquipamentConsumables)
                .WithOne(e => e.Equipament);

            modelBuilder.Entity<EquipamentSpecifications>()
                .HasOne(x => x.Equipament)
                .WithMany(x => x.EquipamentSpecifications);

            modelBuilder.Entity<EquipamentSpecifications>()
                .HasOne(x => x.Specification)
                .WithMany(x => x.EquipamentSpecifications);

            modelBuilder.Entity<EquipamentConsumable>()
                .HasOne(x => x.Equipament)
                .WithMany(x => x.EquipamentConsumables);

            modelBuilder.Entity<EquipamentConsumable>()
                .HasOne(x => x.Consumable)
                .WithMany(x => x.EquipamentConsumables);

            modelBuilder.Entity<EquipmentRelationship>()
                .HasMany(c => c.equipmentRelationshipEquipment)
                .WithOne(c => c.EquipmentRelationship);

            modelBuilder.Entity<ClientSpecification>()
                .HasOne(x => x.Client)
                .WithMany(x => x.ClientSpecifications);

            modelBuilder.Entity<PriceTable>()
                .HasOne(c => c.Equipment)
                .WithMany(e => e.PriceTable);

            modelBuilder.Entity<CalendarReportResponse>().HasNoKey();

            modelBuilder.Entity<CalendarViewResponse>().HasNoKey();


            base.OnModelCreating(modelBuilder);
        }

        public async Task<bool> Commit()
        {
            var success = await SaveChangesAsync() > 0;

            return success;
        }
    }
}
