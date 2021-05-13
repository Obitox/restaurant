using System.Threading.Tasks;
using Restaurant.Domain.ApiModels;

namespace Restaurant.Domain.Services
{
    public interface IHomeService
    {
        Task<Home> HomeData();
    }
}