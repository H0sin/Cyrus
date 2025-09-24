using Cyrus.Utilities.Resources;
using Microsoft.Extensions.Logging;

namespace Cyrus.Utilities;

public class CyrusService(ITranslator translator, ILoggerFactory loggerFactory)
{
    public readonly ITranslator Translator = translator;
    public readonly ILoggerFactory LoggerFactory = loggerFactory;
}