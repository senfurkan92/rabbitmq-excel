using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Context
{
	public class AppDbContext : IdentityDbContext<AppUser,AppRole, int>
	{
		public AppDbContext()
		{

		}

		public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
		{

		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("data source=.;database=RabbitMqExcel;trusted_connection=true;encrypt=false");
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<UserFile>().HasKey(x => x.Id);
			builder.Entity<UserFile>().Property(x => x.PublicPath).IsRequired(false);
			builder.Entity<UserFile>().Property(x => x.FileName).IsRequired(true);
            builder.Entity<UserFile>().Property(x => x.FilePath).IsRequired(true);
            builder.Entity<UserFile>().Property(x => x.AddedDate).IsRequired(true).HasDefaultValue(DateTime.Now);
			builder.Entity<UserFile>().Property(x => x.CreatedDate).IsRequired(false);
			builder.Entity<UserFile>().Ignore(x => x.GetCreatedDate);
			builder.Entity<UserFile>().Property(x => x.FileStatus).IsRequired(true).HasDefaultValue(FileStatus.Existing);
			builder.Entity<UserFile>().HasOne(x => x.User).WithMany(x => x.Files).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull).IsRequired(false);

			var hasher = new PasswordHasher<AppUser>();

			builder.Entity<AppUser>().HasData(new AppUser
			{
				Email= "senfurkan92@gmail.com",
				Id= 1,
				PhoneNumber= "1234567890",
				UserName= "senfurkan92",
				PasswordHash = hasher.HashPassword(null, "emintek!2022!"),
			});

			builder.Entity<Product>().HasNoKey();
		}

		
		public DbSet<UserFile> UserFiles { get; set; }

		public DbSet<Product> Products { get; set; }

	}

	public class AppUser : IdentityUser<int> 
	{
		public virtual ICollection<UserFile> Files { get; set; }

	}

	public class AppRole : IdentityRole<int>
	{

	}
}
