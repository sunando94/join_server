using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace test.Models
{
   public class SubCategory
    {
       [Key]
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public virtual  Category Category { get; set; }
    }
}
