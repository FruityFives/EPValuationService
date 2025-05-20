using System.Threading.Tasks;
using ValuationServiceAPI.Models;

namespace ValuationServiceAPI.Services
{
    public interface IRabbitMqPublisher
    {
        Task PublishAsync(ItemAssessmentDTO dto);
    }
}
