using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using test.Controllers;
using test.Models;
using System.Web.Http;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Net;
using test.Infrastructure;
using test.Models;
using System.Collections.Generic;
namespace UnitTestProject1
{
    [TestClass]
    public class EndpointTest
    {
        [TestMethod]
        public void PostnewUser()
        {
            List<string> s = new List<string>();
            s.Add("fDDfKwbsddg:APA91bFzsuZjLZsAl0ljc8XPv65TfoHjkZFT5JdrgBfNEGxTIihQv8Ap7aFRR12MjTXDTRx4d2TZLrVBFAjM4PFoh7qfEUkcMV3eGD5BTcV-kMDlOiGJwEx0AkCseWD3v4bR81jF92_O");
            string a = new AndroidGCMPushNotification().SendNotification(new DownStream()
            {
                registration_ids = s,
                priority = "High",
                notification = new Notification()
                {
                    title = Constants.delete_event_title,
                    body = Constants.delete_event_body,
                    icon = Constants.icon,
                    eventID=1

                }
            });
        }

        [TestMethod]
        public void GCMTest()
        {
            
        }

    }
}
