using Core.Models;

namespace Core.Interfaces;

public interface IMessagingService
{
    MessageTemplate ResolveTemplateToMessage(MessageTemplate template, MapsetSubmission submission);
    Task<bool> SendMessageTemplate(MessageTemplate resolvedTemplate, int userId, int delay = 250);
    Task<bool[]> SendBulkMessageTemplate(MessageTemplate template, MapsetSubmission[] submissions, int delay = 750);
}