using Core.Interfaces;
using Core.Models;
using Infrastructure.Utils;

namespace Infrastructure.Processing;

public class OsuMessagingService(IOsuClient osuClient) : IMessagingService
{
    public MessageTemplate ResolveTemplateToMessage(MessageTemplate template, MapsetSubmission submission)
    {
        var values = new Dictionary<string, string?>
        {
            { "TemplateName", template.Name },
            { "Artist", submission.Artist },
            { "Title", submission.Title },
            { "Status", submission.Status.ToString().ToUpper() }
        };

        var templateMessages = template.Messages.Select(m => Interpolator.Resolve(m, values)).ToList();

        return new MessageTemplate(template.Name, templateMessages);
    }

    public async Task<bool> SendMessageTemplate(MessageTemplate resolvedTemplate, int userId,
        int delay = 250)
    {
        foreach (var message in resolvedTemplate.Messages)
        {
            try
            {
                await osuClient.SendToUser(userId, message);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"SendMessageTemplate: {e.Message}");
            }

            await Task.Delay(delay);
        }

        return true;
    }

    public async Task<bool[]> SendBulkMessageTemplate(MessageTemplate template, MapsetSubmission[] submissions,
        int delay = 750)
    {
        List<bool> results = [];
        foreach (var submission in submissions)
        {
            var resolvedSubmission = ResolveTemplateToMessage(template, submission);

            var sentMessage = await SendMessageTemplate(resolvedSubmission, submission.UserId);

            results.Add(sentMessage);
            await Task.Delay(delay);
        }

        return [.. results];
    }
}