using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net;
using System.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Collections.Specialized;
using test.Models;
using Newtonsoft.Json;
using System.Web.Configuration;

namespace test.Infrastructure
{
  
   
public class AndroidGCMPushNotification
{
	public AndroidGCMPushNotification()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public string SendNotification(DownStream d)
    {
     // string s= ConfigurationManager.AppSettings[1];
       // string GoogleAppID = System.Configuration.ConfigurationManager.AppSettings["MyAppSetting"];    
        string GoogleAppID = Constants.googleAppID;
        var SENDER_ID = Constants.Senderid;
        //var value = message;
        WebRequest tRequest;
        tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
        tRequest.Method = "post";
        tRequest.ContentType = "application/json";
        tRequest.Headers.Add(string.Format("Authorization:key={0}", GoogleAppID));

        tRequest.Headers.Add(string.Format("Sender:id={0}", SENDER_ID));

      //  DownStream d = new DownStream() { to = deviceId, notification = new Notification() { body="MEssage",title="try",icon="icon"} };

        dynamic postData = JsonConvert.SerializeObject(d);
        tRequest.Timeout = 20000;
        Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
        tRequest.ContentLength = byteArray.Length;
       
            Stream dataStream = tRequest.GetRequestStream();

        dataStream.Write(byteArray, 0, byteArray.Length);
        dataStream.Close();

        WebResponse tResponse = tRequest.GetResponse();

        dataStream = tResponse.GetResponseStream();

        StreamReader tReader = new StreamReader(dataStream);

        String sResponseFromServer = tReader.ReadToEnd();
        
        tReader.Close();
        dataStream.Close();
        tResponse.Close();
        return sResponseFromServer;
    }
}
}