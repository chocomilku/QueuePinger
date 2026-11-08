using Core.Models;

namespace Core.Interfaces;

public interface IMessagingService
{
    MessageTemplate ResolveTemplateToMessage(MessageTemplate template, MapsetSubmission submission);
    bool SendMessageTemplate(MessageTemplate resolvedTemplate, MapsetSubmission submission);
    (bool, string[]) SendBulkMessageTemplate(MessageTemplate template, MapsetSubmission submission);
}