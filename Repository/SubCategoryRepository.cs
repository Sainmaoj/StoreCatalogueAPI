using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using StoreCatalogueAPI.Models;
using StoreCatalogueDA.DataObject;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StoreCatalogueDA.Repository
{
    public class SubCategoryRepository :DocumentDB
    {
        public SubCategoryRepository() : base("TestDb", "SubCategories") { }

        public Task<IOrderedQueryable<SubCategory>> GetAllSubCategorysAsync()
        {
            return Task<IOrderedQueryable<SubCategory>>.Run(() => Client.CreateDocumentQuery<SubCategory>(Collection.DocumentsLink));
        }

        public Task<SubCategory> GetSubCategoryAsync(Guid id)
        {
            return Task<SubCategory>.Run(() =>
                Client.CreateDocumentQuery<SubCategory>(Collection.DocumentsLink)
                .Where(p => p.Id == id)
                .AsEnumerable()
                .FirstOrDefault());
        }


        public Task<ResourceResponse<Document>> CreateSubCategoryAsync(SubCategory SubCategory)
        {
            return Client.CreateDocumentAsync(Collection.DocumentsLink, SubCategory);
        }

        public Task<ResourceResponse<Document>> UpdateSubCategoryAsync(SubCategory SubCategory)
        {
            var doc = Client.CreateDocumentQuery<Document>(Collection.DocumentsLink)
                .Where(d => d.Id == SubCategory.Id.ToString())
                .AsEnumerable()
                .FirstOrDefault();

            return Client.ReplaceDocumentAsync(doc.SelfLink, SubCategory);
        }

        public Task<ResourceResponse<Document>> DeleteSubCategoryAsync(Guid id)
        {
            var doc = Client.CreateDocumentQuery<Document>(Collection.DocumentsLink)
                .Where(d => d.Id == id.ToString())
                .AsEnumerable()
                .FirstOrDefault();
            return Client.DeleteDocumentAsync(doc.SelfLink);
        }
        public bool CheckCategoryHasSubCategories(Guid id)
        {
            return Client.CreateDocumentQuery<SubCategory>(Collection.DocumentsLink)
                .Where(p => p.CategoryId == id)
                .Count() > 0;
        }
    }
}
