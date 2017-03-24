using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using test.Models;

namespace test.Controllers
{
    [RoutePrefix("api")]
    public class UsersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        // GET api/Users
        /// <summary>
        /// Returns the list of all the users
        /// </summary>
        /// <returns>List of Users</returns>
        
        [Route("Users")]
        [HttpGet]
        [ResponseType(typeof(UserGetBindingModel))]
        public IHttpActionResult GetUsers()
        {
           
            List<UserGetBindingModel> userlist = db.Users.Select(x => new UserGetBindingModel
            {
                about = x.about,
                createdEvents = x.createdEvents,
                deviceType = x.deviceType,
                email = x.email,
                firstName = x.firstName,
                gcm_id = x.gcm_id
                ,
                joinedEvents = x.joinedEvents,
                lastName = x.lastName,
                preferredCategory = x.preferredCategory.Select(r => new CategoryBindingModel {CategoryName=r.CategoryName,CategoryId=r.CategoryId }).ToList<CategoryBindingModel>(),
                profileid=x.profileid,
                profilepic=x.profilepic,
                watchedEvents=x.watchedEvents

            })
                
                .ToList<UserGetBindingModel>();
       

             return Ok(userlist);
            
        }
        
        // GET api/Users/5
       /// <summary>
       /// get a single user
       /// </summary>
       /// <param name="profileid">facebook id</param>
       /// <returns></returns>
        [ResponseType(typeof(User))]
        [HttpGet]
        [Route("Users/{profileid}",Name="GetUserByID")]
        public async Task<IHttpActionResult> GetUser(long profileid)
        {
            User user = await db.Users.Where(x=>x.profileid==profileid).FirstOrDefaultAsync<User>();
            db.Entry(user).Collection(s => s.createdEvents).Load();
            db.Entry(user).Collection(s => s.preferredCategory).Load();
            db.Entry(user).Collection(s => s.joinedEvents).Load();
            db.Entry(user).Collection(s => s.watchedEvents).Load();
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT api/Users/5

        /// <summary>
        /// Modify a valid user
        /// </summary>
        /// <param name="profileid">facebook profile id</param>
        /// <param name="user">update model</param>
        /// <returns>Updated user object</returns>
        [ResponseType(typeof(User))]
        [Route("Users/{profileid}")]
        [HttpPut]
        public async Task<IHttpActionResult> PutUser(long profileid, UserUpdateBindingModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            User tobeUpdatedUser = null;

            if (UserExists((long)profileid))
            {

                 tobeUpdatedUser = await db.Users.Where(x => x.profileid == profileid).FirstOrDefaultAsync<User>();

            } if (user.lastName != null)
            tobeUpdatedUser.lastName = user.lastName;
            if(user.firstName!=null)
                        tobeUpdatedUser.firstName = user.firstName;
            if (user.email != null)
                tobeUpdatedUser.email = user.email;
            if(user.about!=null)
            tobeUpdatedUser.about = user.about;
            if(user.profilepic!=null)
            tobeUpdatedUser.profilepic = user.profilepic;
            if(user.deviceType!=null)
            tobeUpdatedUser.deviceType = user.deviceType;
            db.Entry(tobeUpdatedUser).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists((long)profileid))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("GetUserByID", new { profileid = tobeUpdatedUser.profileid }, tobeUpdatedUser);
        }
        
        // POST api/Users
        /// <summary>
        /// Create a new User
        /// </summary>
        /// <param name="user">user model</param>
        /// <returns>Object of the created user</returns>
        [ResponseType(typeof(User))]
        [Route("Users")]
        [HttpPost]
        public async Task<IHttpActionResult> PostUser(UserCreateBindingModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (UserExists(user.profileid))
                return BadRequest("User Exists, profileid should be unique");
            User newUser = new User()
            {
                about = user.about,
                deviceType = user.deviceType,
                firstName = user.firstName,
                gcm_id = user.gcm_id,
                email=user.email,
                lastName = user.lastName,
                profileid = user.profileid,
                profilepic = user.profilepic,
                joinedEvents=new List<Event>(),
                watchedEvents=new List<Event>(),
                preferredCategory=new List<Category>()


            };
            db.Users.Add(newUser);
            try
            {

                await db.SaveChangesAsync();
            }
            catch (Exception ex) { throw;  }

          return CreatedAtRoute("GetUserByID", new { profileid = newUser.profileid }, newUser);
        }

        // DELETE api/Users/5
        /// <summary>
        /// Delete an existing user
        /// </summary>
        /// <param name="profileid">profile of an existing user</param>
        /// <returns>Deleted user Object</returns>
        [ResponseType(typeof(User))]
        [HttpDelete]
        [Route("Users/{profileid}")]
        public async Task<IHttpActionResult> DeleteUser(long profileid)
        {
            User user = await db.Users.Where(x=>x.profileid==profileid).FirstOrDefaultAsync<User>();

            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return Ok(user);
        }


         
        // PUT api/Users/addpreferedcategory/4
        /// <summary>
        /// Add preferred Category to an User
        /// </summary>
        /// <param name="profileid">Profile id of an existing user</param>
        /// <param name="categoryName">The category name to add</param>
        /// <returns>User object</returns>
        [HttpPut]
        [Route("Users/addpreferedcatgory/{profileid}")]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> AddPreferedCategory(long profileid,string categoryName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            User user = await db.Users.Where(x => x.profileid == profileid).FirstOrDefaultAsync<User>();
            db.Entry(user).Collection(s => s.createdEvents).Load();
            db.Entry(user).Collection(s => s.preferredCategory).Load();
            db.Entry(user).Collection(s => s.joinedEvents).Load();
            db.Entry(user).Collection(s => s.watchedEvents).Load();
            ICollection<Category> preferredcategory = user.preferredCategory;
            if (user.preferredCategory == null)
                preferredcategory = new List<Category>();
            Category add = await db.Categories.Where(x => x.CategoryName == categoryName).FirstOrDefaultAsync<Category>();
            preferredcategory.Add(add);
            user.preferredCategory = preferredcategory;
            db.Entry(user).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists((long)profileid))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("GetUserByID", new { profileid = user.profileid }, user);

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(long profileid)
        {
            return db.Users.Count(e => e.profileid == profileid) > 0;
        }
    }
}