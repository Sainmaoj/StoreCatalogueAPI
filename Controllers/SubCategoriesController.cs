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
    public class SubCategoriesController : ApiController
    {
        // GET: api/SubCategories
        public async Task<IOrderedQueryable<SubCategory>> GetSubCategories()
        {
            return await StoreCatalogueDataContext._subCatRepo.GetAllSubCategorysAsync();
        }

        // GET: api/SubCategories/5
        [ResponseType(typeof(SubCategory))]
        public async Task<IHttpActionResult> GetSubCategory(Guid id)
        {
            SubCategory subCategory = await StoreCatalogueDataContext._subCatRepo.GetSubCategoryAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return Ok(subCategory);
        }

        // PUT: api/SubCategories/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSubCategory(Guid id, SubCategory subCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != subCategory.Id)
            {
                return BadRequest();
            }

            try
            {
                if(!await checkSubCategoryExists(id))
                    return NotFound();
                bool isvalidCategory = await checkValidCategory(subCategory.CategoryId);
                if (!isvalidCategory)
                    return BadRequest("Invalid Category");
                await StoreCatalogueDataContext._subCatRepo.UpdateSubCategoryAsync(subCategory);
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

        // POST: api/SubCategories
        [ResponseType(typeof(SubCategory))]
        public async Task<IHttpActionResult> PostSubCategory(SubCategory subCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            try
            {
                bool isvalidCategory = await checkValidCategory(subCategory.CategoryId);
                if (!isvalidCategory)
                    return BadRequest("Invalid Category");
                await StoreCatalogueDataContext._subCatRepo.CreateSubCategoryAsync(subCategory);
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

            return CreatedAtRoute("DefaultApi", new { id = subCategory.Id }, subCategory);
        }

        // DELETE: api/SubCategories/5
        [ResponseType(typeof(SubCategory))]
        public async Task<IHttpActionResult> DeleteSubCategory(Guid id)
        {
            SubCategory subC = await StoreCatalogueDataContext._subCatRepo.GetSubCategoryAsync(id);
            bool hasProducts = StoreCatalogueDataContext._prodRepo.CheckSubCategoryHasProducts(id);
            if (subC == null)
            {
                return NotFound();
            }
            if (hasProducts)
            {
                return Conflict();
            }

            await StoreCatalogueDataContext._subCatRepo.DeleteSubCategoryAsync(id);

            return Ok(subC);
        }
        private async Task<bool> checkSubCategoryExists(Guid id)
        {
            bool exists = false;

            SubCategory subCategory = await StoreCatalogueDataContext._subCatRepo.GetSubCategoryAsync(id);
            if (subCategory != null)
                exists = true;
            return exists;
        }
        private async Task<bool> checkValidCategory(Guid categoryId)
        {
            bool isValid = false;

            Category category = await StoreCatalogueDataContext._catRepo.GetCategoryAsync(categoryId);
            if (category != null)
                isValid = true;
            return isValid;
        }
    }
}