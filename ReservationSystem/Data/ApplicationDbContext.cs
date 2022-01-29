using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ReservationSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        #region TABLE SETUP
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Reservation> Reservations { get; set; }        
        public DbSet<SCTable> SCTables { get; set; }
        public DbSet<SCTimeslot> SCTimeslots { get; set; }
        public DbSet<Sitting> Sittings { get; set; }
        public DbSet<SittingCategory> SittingCategories { get; set; }
        public DbSet<SittingUnit> SittingUnits { get; set; }
        public DbSet<Table> Tables { get; set; }
        #endregion

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region CONFIGURATIONS

            builder.Entity<Customer>(b =>
            {
                b.Property(rc => rc.CustFName).IsRequired();
                b.Property(rc => rc.CustLName).IsRequired();
                b.Property(rc => rc.CustPhone).IsRequired();
                b.Property(rc => rc.CustEmail).IsRequired();
                b.HasOne(p => p.IdentityUser).WithOne().HasForeignKey<Customer>();
            });

            builder.Entity<Reservation>(b =>
            {
                b.Property(r => r.NumOfGuests).IsRequired();
                b.Property(r => r.ExpectedStartTime).HasColumnType("time(0)").IsRequired();
                b.Property(r => r.ExpectedEndTime).HasColumnType("time(0)").IsRequired();
                b.Property(r => r.TimeOfBooking).IsRequired();
                b.Property(r => r.WayOfBooking).IsRequired();
                b.Property(r => r.Status).IsRequired();
            });

            builder.Entity<SCTimeslot>(b =>
            {
                b.Property(t => t.StartTime).HasColumnType("time(0)").IsRequired();
                b.Property(t => t.EndTime).HasColumnType("time(0)").IsRequired();
            });

            builder.Entity<Sitting>(b =>
            {
                b.Property(s => s.Date).HasColumnType("Date").IsRequired();
                b.Property(s => s.Status).IsRequired();
            });

            builder.Entity<SittingCategory>(b =>
            {
                b.HasIndex(sc => sc.Name).IsUnique();
                b.Property(sc => sc.Name).IsRequired();
                b.Property(sc => sc.Capacity).IsRequired();
                b.Property(sc => sc.StartTime).HasColumnType("time(0)").IsRequired();
                b.Property(sc => sc.Duration).HasColumnType("time(0)").IsRequired();
            });

            builder.Entity<SittingUnit>(b =>
            {
                b.Property(s => s.TimeslotId).IsRequired();
                b.Property(s => s.TableId).IsRequired();
                b.Property(s => s.Status).IsRequired();
            });

            builder.Entity<Table>(b =>
            {
                b.Property(t => t.Area).IsRequired();
                b.Property(t => t.Name).IsRequired();                
            });
            #endregion

            #region DATA SEEDING

            builder.Entity<Table>().HasData(
                new { Id = 1, Area = "Main", Name = "M1" },
                new { Id = 2, Area = "Main", Name = "M2" },
                new { Id = 3, Area = "Main", Name = "M3" },
                new { Id = 4, Area = "Main", Name = "M4" },
                new { Id = 5, Area = "Main", Name = "M5" },
                new { Id = 6, Area = "Main", Name = "M6" },
                new { Id = 7, Area = "Main", Name = "M7" },
                new { Id = 8, Area = "Main", Name = "M8" },
                new { Id = 9, Area = "Main", Name = "M9" },
                new { Id = 10, Area = "Main", Name = "M10" },
                new { Id = 11, Area = "Outside", Name = "O1" },
                new { Id = 12, Area = "Outside", Name = "O2" },
                new { Id = 13, Area = "Outside", Name = "O3" },
                new { Id = 14, Area = "Outside", Name = "O4" },
                new { Id = 15, Area = "Outside", Name = "O5" },
                new { Id = 16, Area = "Outside", Name = "O6" },
                new { Id = 17, Area = "Outside", Name = "O7" },
                new { Id = 18, Area = "Outside", Name = "O8" },
                new { Id = 19, Area = "Outside", Name = "O9" },
                new { Id = 20, Area = "Outside", Name = "O10" },
                new { Id = 21, Area = "Balcony", Name = "B1" },
                new { Id = 22, Area = "Balcony", Name = "B2" },
                new { Id = 23, Area = "Balcony", Name = "B3" },
                new { Id = 24, Area = "Balcony", Name = "B4" },
                new { Id = 25, Area = "Balcony", Name = "B5" },
                new { Id = 26, Area = "Balcony", Name = "B6" },
                new { Id = 27, Area = "Balcony", Name = "B7" },
                new { Id = 28, Area = "Balcony", Name = "B8" },
                new { Id = 29, Area = "Balcony", Name = "B9" },
                new { Id = 30, Area = "Balcony", Name = "B10" }
              );
            #endregion
        }
    }
}
