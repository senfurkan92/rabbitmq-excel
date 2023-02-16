using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
	public class Product
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Explanation { get; set; }
		public string Pictures { get; set; }
		public string Spot { get; set; }
		public int ViewCount { get; set; }
		public string Keywords { get; set; }
		public string Url { get; set; }
		public int CategoryId { get; set; }
		public decimal Price { get; set; }
		public string UPCCode { get; set; }
		public string Shipment { get; set; }
		public string Source { get; set; }
		public string ProductType { get; set; }
		public string Brand { get; set; }
		public DateTime CreatedDate { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
		public int Stock { get; set; }
	}
}
