using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Northwind_Console.Model
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        [Required(ErrorMessage = "YO - Enter the name!")] public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
