using StoreCatalogueAPI.Models;
using StoreCatalogueDA.DataObject;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace StoreCatalogueAPI.Controllers
{
    public class CategoriesController : ApiController
    {
        // GET: api/Categories
        public async Task<IOrderedQueryable<Category>> GetCategories()
        {
            return await StoreCatalogueDataContext._catRepo.GetAllCategorysAsync();
        }

        // GET: api/Categories/5
        [ResponseType(typeof(Category))]
        public async Task<IHttpActionResult> GetCategory(Guid id)
        {
            Category category = await StoreCatalogueDataContext._catRepo.GetCategoryAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // PUT: api/Categories/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCategory(Guid id, Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != category.Id)
            {
                return BadRequest();
            }

            try
            {
                if (!await checkCategoryExists(id))
                    return NotFound();
                await StoreCatalogueDataContext._catRepo.UpdateCategoryAsync(category);
            }
            catch (Microsoft.Azure.Documents.DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }

            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Categories
        [ResponseType(typeof(Category))]
        public async Task<IHttpActionResult> PostCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            try
            {
                await StoreCatalogueDataContext._catRepo.CreateCategoryAsync(category);
            }
            catch (Microsoft.Azure.Documents.DocumentClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [ResponseType(typeof(Category))]
        public async Task<IHttpActionResult> DeleteCategory(Guid id)
        {
            Category category = await StoreCatalogueDataContext._catRepo.GetCategoryAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            bool hasSubCats = StoreCatalogueDataContext._subCatRepo.CheckCategoryHasSubCategories(id);
            if (hasSubCats)
            {
                return Conflict();
            }
            await StoreCatalogueDataContext._catRepo.DeleteCategoryAsync(id);

            return Ok(category);
        }

        private async Task<bool> checkCategoryExists(Guid id)
        {
            bool exists = false;

            Category category = await StoreCatalogueDataContext._catRepo.GetCategoryAsync(id);
            if (category != null)
                exists = true;
            return exists;
        }
    }
}