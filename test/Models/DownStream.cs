using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test.Models
{
    public class DownStream
    {
        //public string to { get; set; }
        public List<string> registration_ids { get; set; }
        public string priority { get; set; }
        public Notification notification { get; set; }


    }
}