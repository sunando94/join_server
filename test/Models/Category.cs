using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace test.Models
{
   public class Category
    {
       [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public ICollection<User> Users { get; set; }
        public  List<SubCategory> subcategories { get; set; }

    }
   public class CategoryBindingModel
   {
       
       public int CategoryId { get; set; }
       public string CategoryName { get; set; }

      
      

   }
}
