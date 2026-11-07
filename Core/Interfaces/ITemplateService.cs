using Core.Models;

namespace Core.Interfaces;

public interface ITemplateService
{
    MessageTemplate ReadTemplate(string filePath);
    bool GenerateTemplate(string filePath);
}