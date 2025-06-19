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
    public class SubscriptionRepo : GenericRepository<Subscription>
    {
        public SubscriptionRepo(TheShineDbContext context) : base(context)
        {
        }
        // Additional methods specific to Subscription can be added here
    }
}
