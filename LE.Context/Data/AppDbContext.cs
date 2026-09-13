using LE.Account.Entities;
using LE.Billing.Entities;
using LE.Entities.OrganizationSetup;
using LE.Entities.User;
using LE.Inventory.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LE.Context.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            // Backs the atomic bill-number sequence (UPDATE ... RETURNING) and prevents
            // duplicate counter rows for the same billing key.
            modelBuilder.Entity<BillingSettings>()
                .HasIndex(bs => bs.key)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }

        //billing
        public DbSet<BillingSettings> billing_settings { get; set; }
        public DbSet<DayClose> day_close { get; set; }
        public DbSet<WoodBill> wood_bill { get; set; }
        public DbSet<ChiranSales> chiran_sales { get; set; }
        public DbSet<ChiranSalesDetail> chiran_sales_detail { get; set; }
        public DbSet<WoodBillMembers> wood_bill_members { get; set; }
        public DbSet<WoodBillMemberTransaction> wood_bill_members_transaction { get; set; }
        public DbSet<WoodBillDetail> wood_bill_detail { get; set; }
        public DbSet<CounterSales> counter_sales { get; set; }
        public DbSet<CounterSalesDetail> counter_sales_details { get; set; }
        public DbSet<LE.Billing.Entities.Service> services { get; set; }
        public DbSet<ServiceCategory> service_categories { get; set; }
        public DbSet<Membership> membership { get; set; }
        public DbSet<Member> members { get; set; }
        public DbSet<MembershipValidity> membership_validity { get; set; }
        public DbSet<MemberPunishment> member_punishment { get; set; }
        public DbSet<Tole> tole { get; set; }
        public DbSet<FirewoodSales> firewood_sales { get; set; }
        public DbSet<FirewoodSalesDetail> firewood_sales_detail { get; set; }
        public DbSet<FurnitureCategory> furniture_category { get; set; }
        public DbSet<Furniture> furniture { get; set; }
        public DbSet<FurnitureSales> furniture_sales { get; set; }
        public DbSet<FurnitureSalesDetail> furniture_sales_detail { get; set; }


        //inventory
        public DbSet<WoodType> wood_type { get; set; }
        public DbSet<Purchase> purchase { get; set; }
        public DbSet<StockCategoryPurpose> purpose_category { get; set; }
        public DbSet<StockItemAvailability> stock_item_availability { get; set; }
        public DbSet<StockItem> stock_item { get; set; }
        public DbSet<StockMovement> stock_movement { get; set; }
        public DbSet<StockUnit> stock_unit { get; set; }
        public DbSet<WoodDetails> wood_details { get; set; }
        public DbSet<DamagedWoodDetail> damaged_wood_detail { get; set; }
        public DbSet<Piling> piling { get; set; }


        //account
        public DbSet<LedgerGroup> ledger_group { get; set; }
        public DbSet<Ledger> ledger { get; set; }
        public DbSet<LedgerBalance> ledger_balance { get; set; }
        public DbSet<Payment> payment { get; set; }
        public DbSet<Receipt> receipt { get; set; }
        public DbSet<Transaction> transaction { get; set; }
        public DbSet<TransactionDetail> transaction_detail { get; set; }
        public DbSet<AccountSettings> account_settings { get; set; }
        public DbSet<FiscalYear> fiscal_year { get; set; }
        public DbSet<LedgerSetup> ledger_setup { get; set; }

        //user
        public DbSet<Authentication> authentications { get; set; }
        public DbSet<DynamicMenu> dynamic_menus { get; set; }
        public DbSet<LoginSession> login_sessions { get; set; }
        public DbSet<Module> modules { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<RolePermissionMap> role_permission_maps { get; set; }
        public DbSet<LE.Entities.User.User> users { get; set; }
        public DbSet<UserRole> user_roles { get; set; }

        //org setup
        public DbSet<OrganizationSetup> organization_setup { get; set; }
    }
}
