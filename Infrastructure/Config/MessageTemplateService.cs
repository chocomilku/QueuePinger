using System.Text.Json.Nodes;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Config;

public class MessageTemplateService(IJsonIoActions<MessageTemplate> jsonIoActions) : ITemplateService
{
    public MessageTemplate ReadTemplate(string filePath)
    {
        return jsonIoActions.ReadFile(filePath);
    }

    public bool GenerateTemplate(string filePath)
    {
        var data = new JsonObject
        {
            ["name"] = "",
            ["messages"] = new JsonArray()
        };

        return jsonIoActions.GenerateFile(filePath, data);
    }
}