using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;

namespace test.Models
{
   public class Event

    {  
        public Event()
    {
        this.joinedUser = new HashSet<User>();
        this.watchUser = new HashSet<User>();
    }
       
       [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int eventId { get; set; }
        public string eventName { get; set; }
        public int currentParticipant { get; set; }
        public string eventDescription { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
       
        public int minParticipants { get; set; }
        public int maxParticipants { get; set; }
        public Location location { get; set; }
        public SubCategory eventCategory { get; set; }
        public string Gender { get; set; }
        public Age age { get; set; }
        public decimal cost { get; set; }
        public virtual  User createdUser { get; set; }
        public virtual  ICollection<User> joinedUser { get; set; }
        public virtual  ICollection<User> watchUser { get; set; }
        public string subCategoryName { get; set; }
        public string shortaddress { get; set; }




    }
   public class EventBindModel
   {
      
       public string eventName { get; set; }
       public string eventDescription { get; set; }
       public string startDate { get; set; }
       public string endDate { get; set; }
      
       public int minParticipants { get; set; }
       public int maxParticipants { get; set; }
       public double latitude { get; set; }
       public double longitude { get; set; }
       public SubCategory subCategory { get; set; }
       public string subCategoryName { get; set; }
       public string Gender { get; set; }
       public Age age { get; set; }
       public decimal cost { get; set; }
       public long createdUserProfileId { get; set; }
   




   }
   public class EventCreationBindModel
   {

       public string eventName { get; set; }
       public string eventDescription { get; set; }
       public string startDate { get; set; }
       public string endDate { get; set; }

       public int minParticipants { get; set; }
       public int maxParticipants { get; set; }
       public double latitude { get; set; }
       public double longitude { get; set; }
       
       public string subCategoryName { get; set; }
       public string Gender { get; set; }
       public Age age { get; set; }
       public decimal cost { get; set; }
       public long createdUserProfileId { get; set; }





   }



}
