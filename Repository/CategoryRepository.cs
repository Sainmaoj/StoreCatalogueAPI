using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using StoreCatalogueAPI.Models;
using StoreCatalogueDA.DataObject;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StoreCatalogueDA.Repository
{
    public class CategoryRepository : DocumentDB
    {
        public CategoryRepository() : base("TestDb", "Categories") { }

        public Task<IOrderedQueryable<Category>> GetAllCategorysAsync()
        {
            return Task<IOrderedQueryable<Category>>.Run(() =>
                Client.CreateDocumentQuery<Category>(Collection.DocumentsLink));
        }

        public Task<Category> GetCategoryAsync(Guid id)
        {
            return Task<Category>.Run(() =>
                Client.CreateDocumentQuery<Category>(Collection.DocumentsLink)
                .Where(p => p.Id == id)
                .AsEnumerable()
                .FirstOrDefault());
        }

        public Task<ResourceResponse<Document>> CreateCategoryAsync(Category Category)
        {
            return Client.CreateDocumentAsync(Collection.DocumentsLink, Category);
        }

        public Task<ResourceResponse<Document>> UpdateCategoryAsync(Category Category)
        {
            var doc = Client.CreateDocumentQuery<Document>(Collection.DocumentsLink)
                .Where(d => d.Id == Category.Id.ToString())
                .AsEnumerable()
                .FirstOrDefault();

            return Client.ReplaceDocumentAsync(doc.SelfLink, Category);
        }

        public Task<ResourceResponse<Document>> DeleteCategoryAsync(Guid id)
        {
            var doc = Client.CreateDocumentQuery<Document>(Collection.DocumentsLink)
                .Where(d => d.Id == id.ToString())
                .AsEnumerable()
                .FirstOrDefault();

            return Client.DeleteDocumentAsync(doc.SelfLink);
        }
    }
}
