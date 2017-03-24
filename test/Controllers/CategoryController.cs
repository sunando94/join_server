using System;
using System.Collections.Generic;
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
    public class CategoryController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        /// <summary>
        /// Get the list of all Categories
        /// </summary>
        /// <returns>List of Category Objects</returns>

        [Route("Category")]
        // GET api/Category
            
        public IQueryable<Category> GetCategories()
        {
            return db.Categories;
        }


        /// <summary>
        /// Get a single Category Object by ID
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category Object</returns>
        [Route("Category/{id}")]
        // GET api/Category/5
        [ResponseType(typeof(Category))]
        public async Task<IHttpActionResult> GetCategory(int? id)
        {
            Category category = await db.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

       
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CategoryExists(int id)
        {
            return db.Categories.Count(e => e.CategoryId == id) > 0;
        }
    }
}