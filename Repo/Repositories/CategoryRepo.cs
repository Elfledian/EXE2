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
    }

}
