using DAL.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
	public class UserFile
	{
		public int Id { get; set; }

		public string? PublicPath { get; set; }

		public string FileName { get; set; }

        public string FilePath { get; set; }

        public DateTime AddedDate { get; set; }

		public DateTime? CreatedDate { get; set; }

		public FileStatus FileStatus { get; set; }

		// relation
		public int? UserId { get; set; }

		public AppUser? User { get; set; }

		// not mapped
		[NotMapped]
		public string GetCreatedDate => CreatedDate is not null ? CreatedDate.Value.ToString("yyyy-MM-dd HH:mm") : "-";
	}

	public enum FileStatus { 
		Existing = 1, Existed, Failed
	}
}
