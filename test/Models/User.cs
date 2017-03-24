using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace test.Models
{
   
    public class User
    {
         public User()
  {
    watchedEvents = new HashSet<Event>();
    joinedEvents = new HashSet<Event>();
  }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int userId { get; set; }
        [Index(IsUnique = true)]
        public long profileid { get; set; }
        public string firstName { get; set; }
        [DataType(DataType.EmailAddress,ErrorMessage="invalid Email")]
        public string email { get; set; }
        public string lastName { get; set; }
        public string profilepic { get; set; }
        public string about { get; set; }
        public  virtual ICollection<Category> preferredCategory { get; set; }
        public  virtual ICollection<Event> watchedEvents { get; set; }
        public virtual  ICollection<Event> joinedEvents { get; set; }
        public  virtual ICollection<Event>  createdEvents{ get; set; }
        public string gcm_id { get; set; }
        public string deviceType { get; set; }
        

    }
    public class UserCreateBindingModel
    {
        public long profileid { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string profilepic { get; set; }
        public string about { get; set; }
        public string gcm_id { get; set; }
        public string deviceType { get; set; }
        

    }
    public class UserGetBindingModel
    {
        
            public long profileid { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string email { get; set; }
            public string profilepic { get; set; }
            public string about { get; set; }
            public string gcm_id { get; set; }
            public string deviceType { get; set; }

            public virtual ICollection<CategoryBindingModel> preferredCategory { get; set; }
            public virtual ICollection<Event> watchedEvents { get; set; }
            public virtual ICollection<Event> joinedEvents { get; set; }
            public virtual ICollection<Event> createdEvents { get; set; }
        
    }

    public class UserUpdateBindingModel
    {
        public long profileid { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }

        public string email { get; set; }
        public string profilepic { get; set; }
        public string about { get; set; }
        public string deviceType { get; set; }

    }
}