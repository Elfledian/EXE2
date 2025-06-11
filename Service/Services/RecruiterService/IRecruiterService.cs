using Repo.Entities;

namespace Service.Services.RecruiterService
{
    public interface IRecruiterService
    {
        Task<Recruiter> GetByUserId(Guid userId);
    }
}