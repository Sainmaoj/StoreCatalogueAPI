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
    public class ProductsController : ApiController
    {
        // GET: api/Products
        public Task<IOrderedQueryable<Product>> GetProducts()
        {
            return StoreCatalogueDataContext._prodRepo.GetAllProductsAsync();
        }

        // GET: api/Products/5
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> GetProduct(Guid id)
        {
            Product product = await StoreCatalogueDataContext._prodRepo.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Products/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProduct(Guid id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.Id)
            {
                return BadRequest();
            }


            try
            {
                if (!await checkProductExists(id))
                    return NotFound();
                bool isvalidSubCategory = await checkValidSubCategory(product.SubCategoryId);
                if (!isvalidSubCategory)
                    return BadRequest("Invalid Sub Category");
                await StoreCatalogueDataContext._prodRepo.UpdateProductAsync(product);
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

        // POST: api/Products
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            try
            {
                bool isvalidSubCategory = await checkValidSubCategory(product.SubCategoryId);
                if(!isvalidSubCategory)
                    return BadRequest("Invalid Sub Category");
                await StoreCatalogueDataContext._prodRepo.CreateProductAsync(product);
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

            return CreatedAtRoute("DefaultApi", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> DeleteProduct(Guid id)
        {
            Product product = await StoreCatalogueDataContext._prodRepo.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            await StoreCatalogueDataContext._prodRepo.DeleteProductAsync(id);
            return Ok(product);
        }

        private async Task<bool> checkValidSubCategory(Guid subCategoryId)
        {
            bool isValid = false;

            SubCategory subCategory = await StoreCatalogueDataContext._subCatRepo.GetSubCategoryAsync(subCategoryId);
            if (subCategory != null)
                isValid = true;
            return isValid;
        }

        private async Task<bool> checkProductExists(Guid id)
        {
            bool exists = false;

            Product product = await StoreCatalogueDataContext._prodRepo.GetProductAsync(id);
            if (product != null)
                exists = true;
            return exists;
        }
    }
}