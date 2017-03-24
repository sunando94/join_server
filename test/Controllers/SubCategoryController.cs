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
    public class SubCategoryController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        /// <summary>
        /// Get all sub categories
        /// </summary>
        /// <returns>List of SubCategory Object</returns>
        [Route("SubCategory")]
        // GET api/SubCategory
        public List<SubCategory> GetSubCategories()
        {
           return db.SubCategories.ToList();
        }
        /// <summary>
        /// Get subcategory by id
        /// </summary>
        /// <param name="id">ID of the subcategroy</param>
        /// <returns>returns a subcategory object</returns>
        [Route("SubCategory/{id}")]
        // GET api/SubCategory/5
        [ResponseType(typeof(SubCategory))]
        public async Task<IHttpActionResult> GetSubCategory(int? id)
        {
            SubCategory subcategory = await db.SubCategories.FindAsync(id);
            if (subcategory == null)
            {
                return NotFound();
            }

            return Ok(subcategory);
        }

       

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SubCategoryExists(int id)
        {
            return db.SubCategories.Count(e => e.SubCategoryId == id) > 0;
        }
    }
}