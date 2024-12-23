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
		public DbSet<Tag> Tag { get; set; }
		public DbSet<RestaurantTag> RestaurantTag { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	    
	}
}
