using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using System.Web.Http.Description;
using test.Models;
using System.Data.Entity.Infrastructure;
using System.Device.Location;
using test.Infrastructure;
using Geocoding.Google;
using Geocoding;


namespace test.Controllers
{
    [RoutePrefix("api")]
    public class EventController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        
        // GET api/event
        /// <summary>
        /// Get the list of Events
        /// </summary>
        /// <returns>List of Event Object</returns>
        [Route("Events")]
        [ResponseType(typeof(Event))]
        public IQueryable<Event> Get()
        {
            return db.Events.Include(r=>r.createdUser).Include(r=>r.joinedUser).Include(r=>r.watchUser);
        }


        /// <summary>
        /// Get List of Events by Age range
        /// </summary>
        /// <param name="startage">The starting age</param>
        /// <param name="endage">The ending age</param>
        /// <returns>List of Event Object</returns>

        [Route("Events/{startage}/{endage}")]
        [HttpGet]
        [ResponseType(typeof(Event))]
        public IQueryable<Event> Getbyage(int startage,int endage)
        {
            return db.Events.Where(r=>r.age.startage>=startage && r.age.endage<=endage);
        }
        

        /// <summary>
        /// Get list of Events by gender
        /// </summary>
        /// <param name="gender">Gender can be either Male/Female/Both</param>
        /// <returns>List of Events</returns>
        [HttpGet]
        [Route("Events/getbygender")]
        [ResponseType(typeof(Event))]
        public IQueryable<Event> Getbygender(string gender)
        {
            return db.Events.Where(r => r.Gender==gender);
        }
        


        
        
        // GET api/event/5
        /// <summary>
        /// Get Event by id
        /// </summary>
        /// <param name="id">Event id</param>
        /// <returns></returns>
        [Route("Events/{id}",Name="GetEventById")]
        [ResponseType(typeof(Event))]
       
        public async Task<IHttpActionResult> Get(int id)
        {
            Event eventobj = await db.Events.Include(r=>r.joinedUser).Include(r=>r.watchUser).Include(r=>r.createdUser).Where(r=>r.eventId==id).FirstAsync();
            if (eventobj == null)
                return NotFound();
            return Ok(eventobj);

         
            
        }
       /// <summary>
       /// Get list of Events by location
       /// </summary>
       /// <param name="latitude">latitude</param>
       /// <param name="longitude">longitude</param>
       /// <param name="radius">the radius of search in kilometers</param>
       /// <returns>List of Event Object</returns>
        [HttpGet]
        [Route("Events/{radius:int?}")]
        public IHttpActionResult Get(double latitude,double longitude,int radius=20)
        {
            test.Models.Location location = new test.Models.Location() { latitude = latitude, longitude = longitude };
            List<Event> returnevents= new List<Event>();
            List<Event> eventlist =  db.Events.ToList<Event>();
            foreach(Event events in eventlist)
            {
                if(getDistance(location,events.location)<=radius)
                {
                    returnevents.Add(events);
                }
            }
            return Ok(returnevents);
                

             

        }
        // POST api/event
        /// <summary>
        /// Creates an Event 
        /// </summary>
        /// <param name="eventmodel">The Event Data</param>
        /// <returns>Created Event Object</returns>
        [ResponseType(typeof(Event))]
        [Route("Events")]
        public async Task<IHttpActionResult> Post(EventCreationBindModel eventmodel)
        {
            Event newEvent=null;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(eventmodel.age.endage<eventmodel.age.startage && eventmodel.age.endage<=0 && eventmodel.age.startage<=0)
            {
                return BadRequest("incorrect start age and end age");
            }
            if(eventmodel.minParticipants<=0)
            {
                return BadRequest("minimum Participants should be more than equal to 1");
            }
            if(eventmodel.maxParticipants<=0)
                return BadRequest("maximum Participants should be more than equal to 1");

            try
            {
                User eventcreatedUser = null;
                if (UserExists(eventmodel.createdUserProfileId))
                    eventcreatedUser = getUser(eventmodel.createdUserProfileId);
                else
                {
                    return NotFound();
                }
                SubCategory eventsubcategory = db.SubCategories.Where(r => r.SubCategoryName == eventmodel.subCategoryName).FirstOrDefault();
                if(eventsubcategory==null)
                {
                    return BadRequest("event subcategory missing");
                }
                IGeocoder geocoder = new GoogleGeocoder() { ApiKey =  Constants.googleAppID };
                IEnumerable<Address> addresses=geocoder.ReverseGeocode(eventmodel.latitude,eventmodel.longitude);
                 newEvent = new Event()
                {
                    createdUser = eventcreatedUser,
                    eventName = eventmodel.eventName,
                    currentParticipant=1,
                    eventDescription = eventmodel.eventDescription,
                    startDate = eventmodel.startDate,
                    endDate = eventmodel.endDate,
                    minParticipants = eventmodel.minParticipants,
                    maxParticipants = eventmodel.maxParticipants,
                    location = new test.Models.Location() { latitude = eventmodel.latitude, longitude = eventmodel.longitude },
                    eventCategory = eventsubcategory,
                    subCategoryName = (eventmodel.subCategoryName == "Other") ? eventmodel.subCategoryName : eventsubcategory.SubCategoryName,
                    joinedUser=new List<Models.User>(),
                    watchUser=new List<Models.User>(),
                    Gender = eventmodel.Gender,
                    age = new Age() { startage = eventmodel.age.startage, endage = eventmodel.age.endage },
                    cost = eventmodel.cost,
                    shortaddress= addresses.Select(r=>r.FormattedAddress).First()

                };
                 // notification to all user with preferred category
                 List<string> gcm_ids = await db.Users.Where(r => r.preferredCategory.Contains(newEvent.eventCategory.Category)).Select(r => r.gcm_id).ToListAsync();
                 if (gcm_ids.Count > 0)
                 {
                     new AndroidGCMPushNotification().SendNotification(new DownStream()
                         {
                             registration_ids = gcm_ids,
                             priority = "High",
                             notification = new Notification()
                             {
                                 title = Constants.preferredcategory_event_title,
                                 body = Constants.preferredcategory_event_body,
                                 icon = Constants.icon

                             }
                         });
                 }

               

            }
                catch (Exception e) {
                return BadRequest(e.Message);
            }
            
            newEvent.joinedUser.Add(newEvent.createdUser);
            
            db.Events.Add(newEvent);
           
                await db.SaveChangesAsync();
           
                User user = await db.Users.Where(r => r.profileid == eventmodel.createdUserProfileId).FirstOrDefaultAsync();
                if (user.joinedEvents == null)
                    user.joinedEvents = new List<Event>();
                user.joinedEvents.Add(newEvent);
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
              
            return CreatedAtRoute("GetEventById", new { id = newEvent.eventId }, newEvent);
        }

        // PUT api/event/5
        /// <summary>
        /// Update an existing Event
        /// </summary>
        /// <param name="eventid">Event id of the event to update</param>
        /// <param name="model">The changes required</param>
        /// <returns>Object of Updated Event</returns>
        [Route("Events")]
        [ResponseType(typeof(Event))]
        public async Task<IHttpActionResult> Put(int eventid, EventCreationBindModel model)
        {
            Event eventmodel=null;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (EventExists(eventid))
                eventmodel = GetEvent(eventid);
            if(model.age!=null)
                 eventmodel.age=model.age;
            if(model.cost!=eventmodel.cost && model.cost!=0)
                 eventmodel.cost=model.cost;
            if (model.createdUserProfileId != eventmodel.createdUser.profileid && model.createdUserProfileId != 0)
            {
                if (UserExists(model.createdUserProfileId))
                    eventmodel.createdUser = await db.Users.Where(r => r.profileid == model.createdUserProfileId).FirstOrDefaultAsync();
            }
            if(model.endDate!=eventmodel.endDate)
                eventmodel.endDate=model.endDate;
            if(model.eventDescription!=null)
                eventmodel.eventDescription=model.eventDescription;
            if(model.eventName!=eventmodel.eventName)
                eventmodel.eventName=model.eventName;
            if(model.Gender!=eventmodel.Gender && (model.Gender=="Male" || model.Gender=="Female" || model.Gender=="Both"))
                eventmodel.Gender=model.Gender;
            if(model.latitude!=eventmodel.location.latitude && model.latitude!=0.0)
                eventmodel.location.latitude=model.latitude;
            if(model.longitude!=eventmodel.location.longitude && model.longitude!=0.0)
                eventmodel.location.longitude=model.longitude;
            if(model.startDate!=eventmodel.startDate)
                eventmodel.startDate=model.startDate;
            if(model.maxParticipants!=eventmodel.maxParticipants && model.maxParticipants>0)
                eventmodel.maxParticipants=model.maxParticipants;
            if(model.minParticipants!=eventmodel.minParticipants && model.minParticipants>0)
                eventmodel.minParticipants=model.minParticipants;
            if(model.subCategoryName!=null)
            {
                eventmodel.eventCategory= await db.SubCategories.Where(r=>r.SubCategoryName==model.subCategoryName).FirstOrDefaultAsync<SubCategory>();
                eventmodel.subCategoryName=model.subCategoryName;
            }
            db.Entry(eventmodel).State = EntityState.Modified;
            try{
                await db.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                throw;
            }
            List<string> reg_id=new List<string>();
            foreach(User u in eventmodel.joinedUser)
            {
                reg_id.Add(u.gcm_id);
            }
            foreach(User u in eventmodel.watchUser)
            {
                reg_id.Add(u.gcm_id);
            }
            if (reg_id.Count > 0)
            {
                new AndroidGCMPushNotification().SendNotification(new DownStream()
                {
                    registration_ids = reg_id,
                    priority = "High",
                    notification = new Notification()
                    {
                        title = Constants.update_event_title,
                        body = Constants.Update_event_body,
                        icon = Constants.icon

                    }
                });
            }

           return CreatedAtRoute("GetEventById",new{ id=eventmodel.eventId},eventmodel);
        }
        
        /// <summary>
        /// Join an existing Event
        /// </summary>
        /// <param name="profileid">The Profile id of an User </param>
        /// <param name="eventid">The Event id of an existing event</param>
        /// <returns>joined event object</returns>
        [Route("Events/joinevent")]
        [ResponseType(typeof(Event))]
        [HttpPut]
        public async Task<IHttpActionResult> joinevent(long profileid,int eventid)
        {
            if(!ModelState.IsValid)
            {
                BadRequest(ModelState);
            }
            if (!UserExists(profileid))
            {
                return NotFound();
            }
           
            if (!EventExists(eventid))
                return NotFound();

            User user = getUser(profileid);

            Event updateevent = GetEvent(eventid);
            updateevent.currentParticipant += 1;
            if (updateevent.joinedUser.Count(r => r.profileid == profileid) <= 0)
                updateevent.joinedUser.Add(user);
            else
                return BadRequest("already joined");
            user.joinedEvents.Add(updateevent);

            db.Entry(updateevent).State = EntityState.Modified;
            


            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            db.Entry(user).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            List<string> reg_ids = new List<string>();
            if(updateevent.currentParticipant>=updateevent.minParticipants)
            {
                foreach(User u in updateevent.joinedUser)
                {
                    reg_ids.Add(u.gcm_id);
                }
                if (reg_ids.Count > 0)
                {
                    new AndroidGCMPushNotification().SendNotification(new DownStream()
                    {
                        registration_ids = reg_ids,
                        priority = "High",
                        notification = new Notification()
                        {
                            title = Constants.minno_event_title,
                            body = Constants.minno_event_body,
                            icon = Constants.icon

                        }
                    });
                }
            }

            return CreatedAtRoute("GetEventById", new { id = updateevent.eventId }, updateevent);

        }
        /// <summary>
        /// Leave an event 
        /// </summary>
        /// <param name="profileid">The Profile id of an User </param>
        /// <param name="eventid">The Event id of an existing event</param>
        /// <returns></returns>
        [Route("Events/leaveevent")]
        [ResponseType(typeof(Event))]
        [HttpPut]
        public async Task <IHttpActionResult> leaveEvent(long profileid,int eventid)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(!UserExists(profileid))
            {
                return NotFound();
            }
            if(!EventExists(eventid))
            {
                return NotFound();
            }
            User user = getUser(profileid);
            Event updateEvent = GetEvent(eventid);
            updateEvent.joinedUser.Remove(user);
            updateEvent.currentParticipant -= 1;
            user.joinedEvents.Remove(updateEvent);
            db.Entry(user).State = EntityState.Modified;
            db.Entry(updateEvent).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                throw;
            }
            #region currentuserslessthanmin
            List<string> gcm_ids = new List<string>();
            if (updateEvent.minParticipants > updateEvent.currentParticipant)
            {
                foreach (User u in updateEvent.joinedUser)
                {
                    gcm_ids.Add(u.gcm_id);
                }
            }
            if (gcm_ids.Count > 0)
            {
                new AndroidGCMPushNotification().SendNotification(new DownStream()
                {
                    registration_ids = gcm_ids,
                    priority = "High",
                    notification = new Notification()
                    {
                        title = Constants.notmin_event_title,
                        body = Constants.notmin_event_body,
                        icon = Constants.icon

                    }
                });
            }
            #endregion

            #region watchedusers
            if (updateEvent.currentParticipant + 1 == updateEvent.maxParticipants)
            {
                List<string> watchedusers = new List<string>();
                foreach (User u in updateEvent.watchUser)
                {
                    watchedusers.Add(u.gcm_id);
                }
                if (gcm_ids.Count > 0)
                {
                    new AndroidGCMPushNotification().SendNotification(new DownStream()
                    {
                        registration_ids = watchedusers,
                        priority = "High",
                        notification = new Notification()
                        {
                            title = Constants.watch_event_title,
                            body = Constants.watch_event_body,
                            icon = Constants.icon

                        }
                    });
                }
            } 
            #endregion

            return CreatedAtRoute("GetEventById", new { id = updateEvent.eventId }, updateEvent);
        }

        /// <summary>
        /// Put user to watch an existing event
        /// </summary>
        /// <param name="profileid">Profile id of an existing user</param>
        /// <param name="eventid">Event id of an existing event</param>
        /// <returns>watched event object</returns>
        [Route("Events/watchevent")]
        [ResponseType(typeof(Event))]
        [HttpPut]
        public async Task<IHttpActionResult> watchevent(long profileid,int eventid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!UserExists(profileid))
            {
                return NotFound();
            }
            if (!EventExists(eventid))
            {
                return NotFound();
            }
            User user = getUser(profileid);
            Event updateEvent = GetEvent(eventid);
            if (updateEvent.currentParticipant == updateEvent.maxParticipants)
            {
                updateEvent.watchUser.Add(user);
                user.watchedEvents.Add(updateEvent);
                db.Entry(user).State = EntityState.Modified;
                db.Entry(updateEvent).State = EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            else
            {
                return BadRequest("Still slots are available");
            }
            return CreatedAtRoute("GetEventById", new { id = updateEvent.eventId }, updateEvent);
        }
        /// <summary>
        /// unwatch an event
        /// </summary>
        /// <param name="profileid">profile id of the user who decides to unwatch an watched event</param>
        /// <param name="eventid">event id of the event</param>
        /// <returns>Unwatched event object</returns>
        [Route("Events/unwatchevent")]
        [ResponseType(typeof(Event))]
        [HttpPut]
        public async Task<IHttpActionResult> unwatchevent(long profileid, int eventid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!UserExists(profileid))
            {
                return NotFound();
            }
            if (!EventExists(eventid))
            {
                return NotFound();
            }
            User user = getUser(profileid);
            Event updateEvent = GetEvent(eventid);
            updateEvent.watchUser.Remove(user);
            user.watchedEvents.Remove(updateEvent);
            db.Entry(user).State = EntityState.Modified;
            db.Entry(updateEvent).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return CreatedAtRoute("GetEventById", new { id = updateEvent.eventId }, updateEvent);
        }
        /// <summary>
        /// Delete an event
        /// </summary>
        /// <param name="eventid">event id of the event to delete</param>
        /// <returns>Deleted event object</returns>
        [HttpDelete]
        [Route("Event")]
        [ResponseType(typeof(Event))]
        public async Task<IHttpActionResult> Delete(int eventid)
        {
            Event eventmodel = null;
            if (EventExists(eventid))
                eventmodel = await db.Events.Include(a=>a.joinedUser).Include(a=>a.watchUser)
                    .Include(a=>a.eventCategory)

                    
                    .Where(r => r.eventId == eventid).FirstOrDefaultAsync<Event>();
            db.Events.Remove(eventmodel);
            
            await db.SaveChangesAsync();
            List<string> registeredUser=new List<string>();
            foreach (User u in eventmodel.joinedUser)
            {
                registeredUser.Add(u.gcm_id);
            }
            if (registeredUser.Count > 0)
            {
                new AndroidGCMPushNotification().SendNotification(new DownStream()
                {
                    registration_ids = registeredUser,
                    priority = "High",
                    notification = new Notification()
                    {
                        title = Constants.delete_event_title,
                        body = Constants.delete_event_body,
                        icon = Constants.icon

                    }
                });
            }
            return Ok(eventmodel);
        }



        #region Helpers
        private Event GetEvent(int eventid)
        {
            return  db.Events.Include(b => b.joinedUser).Include(c => c.watchUser).Where(r => r.eventId == eventid).FirstOrDefault<Event>();
        }

        private Models.User getUser(long profileid)
        {
          
            
                return  db.Users.Where(r => r.profileid == profileid).FirstOrDefault<User>();

        }
        private bool EventExists(int eventid)
        {
            bool flag = db.Events.Count(e => e.eventId == eventid) > 0;
 	        return flag;
        }

        // DELETE api/event/5
        
        private double getDistance(test.Models.Location l1,test.Models.Location l2)
        {
            var sCoord = new GeoCoordinate(l1.latitude, l1.longitude);
            var eCoord = new GeoCoordinate(l2.latitude, l2.longitude);
            double distance = sCoord.GetDistanceTo(eCoord) / 1000;
           // var R = 6371;
           //return Math.Acos(Math.Sin(l1.latitude) * Math.Sin(l2.latitude) + Math.Cos(l1.longitude) * Math.Cos(l2.longitude))*R;
            return distance;
        }
        private bool UserExists(long profileid)
        {
            return db.Users.Count(e => e.profileid == profileid) > 0;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
