using Microsoft.EntityFrameworkCore;
using Repo.Entities;
using Repo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly CategoryRepo _categoryRepo;
        public CategoryService(CategoryRepo categoryRepo)
        {
            _categoryRepo = categoryRepo ?? throw new ArgumentNullException(nameof(categoryRepo));
        }
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepo.GetAll().ToListAsync();
        }
        public void AddSample()
        {
            _categoryRepo.Add5SampleCategories();
        }
    }
}
