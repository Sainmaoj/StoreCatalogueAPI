using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using StoreCatalogueAPI.Models;
using StoreCatalogueDA.DataObject;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StoreCatalogueDA.Repository
{
    public class ProductRepository : DocumentDB
    {
        public ProductRepository() : base("TestDb", "Products") { }

        public Task<IOrderedQueryable<Product>> GetAllProductsAsync()
        {
            return Task<IOrderedQueryable<Product>>.Run(() =>
                Client.CreateDocumentQuery<Product>(Collection.DocumentsLink));
        }

        public Task<Product> GetProductAsync(Guid id)
        {
            return Task<Product>.Run(() =>
                Client.CreateDocumentQuery<Product>(Collection.DocumentsLink)
                .Where(p => p.Id == id)
                .AsEnumerable()
                .FirstOrDefault());
        }
        public bool CheckSubCategoryHasProducts(Guid id)
        {
            return Client.CreateDocumentQuery<Product>(Collection.DocumentsLink)
                .Where(p => p.SubCategoryId == id).Count() > 0;
        }
        public Task<ResourceResponse<Document>> CreateProductAsync(Product Product)
        {
            return Client.CreateDocumentAsync(Collection.DocumentsLink, Product);
        }

        public Task<ResourceResponse<Document>> UpdateProductAsync(Product Product)
        {
            var doc = Client.CreateDocumentQuery<Document>(Collection.DocumentsLink)
                .Where(d => d.Id == Product.Id.ToString())
                .AsEnumerable()
                .FirstOrDefault();

            return Client.ReplaceDocumentAsync(doc.SelfLink, Product);
        }

        public Task<ResourceResponse<Document>> DeleteProductAsync(Guid id)
        {
            var doc = Client.CreateDocumentQuery<Document>(Collection.DocumentsLink)
                .Where(d => d.Id == id.ToString())
                .AsEnumerable()
                .FirstOrDefault();

            return Client.DeleteDocumentAsync(doc.SelfLink);
        }

    }
}

