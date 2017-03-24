using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test.Models
{
    public class Notification
    {
        public Notification()
        { }
        public string title { get; set; }
        public string body { get; set; }
        public string icon { get; set; }
        public int eventID { get; set; }

    }
}