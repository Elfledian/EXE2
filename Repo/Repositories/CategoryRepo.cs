using Repo.Data;
using Repo.Entities;
using Repo.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.Repositories
{
    public class CategoryRepo :GenericRepository<Category>
    {
        public CategoryRepo(TheShineDbContext context) : base(context)
        {
        }
        public void Add5SampleCategories()
        {
            var categories = new List<Category>
            {
                new Category { Title = "Web Development", SubItems = "HTML, CSS, JavaScript" },
                new Category { Title = "Mobile Development", SubItems = "Android, iOS" },
                new Category { Title = "Event Support", SubItems = "Wedding, Seminar, Talkshow" },
                new Category { Title = "Graphic Design", SubItems = "Photoshop, Illustrator" },
                new Category { Title = "Digital Marketing", SubItems = "SEO, Social Media" }
            };
            foreach (var category in categories)
            {
                _context.Categories.Add(category);
            }
            _context.SaveChanges();
        }
    }

}
