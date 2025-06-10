using Repo.Entities;

namespace Service.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoriesAsync();
    }
}