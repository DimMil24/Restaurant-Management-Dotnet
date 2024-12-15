using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restaurant_Manager.Areas.Identity;
using Restaurant_Manager.Models;

namespace Restaurant_Manager.Data
{
	public class ApplicationDbContext : IdentityDbContext<CustomIdentityUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<Product> Product { get; set; }
		public DbSet<CustomerOrder> CustomerOrder { get; set; }
		public DbSet<OrderProduct> OrderProduct { get; set; }
		public DbSet<Restaurant> Restaurant { get; set; }
		public DbSet<Category> Category { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			// modelBuilder.Entity<CustomIdentityUser>(b =>
			// {
			// 	b.ToTable("aspnet_users");
			// });
			//
			// modelBuilder.Entity<IdentityUserClaim<string>>(b =>
			// {
			// 	b.ToTable("aspnet_claims");
			// });
			//
			// modelBuilder.Entity<IdentityUserLogin<string>>(b =>
			// {
			// 	b.ToTable("aspnet_logins");
			// });
			//
			// modelBuilder.Entity<IdentityUserToken<string>>(b =>
			// {
			// 	b.ToTable("aspnet_tokens");
			// });
			//
			// modelBuilder.Entity<IdentityRole>(b =>
			// {
			// 	b.ToTable("aspnet_roles");
			// });
			//
			// modelBuilder.Entity<IdentityRoleClaim<string>>(b =>
			// {
			// 	b.ToTable("aspnet_role_claims");
			// });
			//
			// modelBuilder.Entity<IdentityUserRole<string>>(b =>
			// {
			// 	b.ToTable("aspnet_user_roles");
			// });
		}
	    
	}
}
