using System.Threading.Tasks;

namespace StoreCatalogueAPI.Models
{
    public static class StoreCatalogueDataContext
    {
        public static StoreCatalogueDA.Repository.ProductRepository _prodRepo;
        public static StoreCatalogueDA.Repository.CategoryRepository _catRepo;
        public static StoreCatalogueDA.Repository.SubCategoryRepository _subCatRepo;
        public static Task Initilization { get; private set; }
        static StoreCatalogueDataContext()
        {
            _prodRepo = new StoreCatalogueDA.Repository.ProductRepository();
            _catRepo = new StoreCatalogueDA.Repository.CategoryRepository();
            _subCatRepo = new StoreCatalogueDA.Repository.SubCategoryRepository();
            Initilization = InitializeAsync();
        }
        private static async Task InitializeAsync()
        {
            await _prodRepo.Initilization;
            await _catRepo.Initilization;
            await _subCatRepo.Initilization;
        }
    }
}