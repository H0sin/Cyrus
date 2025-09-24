using Cyrus.Core.Domain.Toolkits.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cyrus.Infrastructure.Data.Sql.Commands.ValueConversions;

public class DescriptionConversion()
    : ValueConverter<Description, string>(c => c.Value, c => Description.FromString(c));