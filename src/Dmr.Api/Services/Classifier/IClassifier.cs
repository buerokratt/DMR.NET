using Dmr.Api.Models;

namespace Dmr.Api.Services.Classifier
{
    public interface IClassifier
    {
        Task Classify(MessagesInput messages);
    }
}
