using Cyrus.Core.Domain.Toolkits.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cyrus.Infrastructure.Data.Sql.Commands.ValueConversions;

public abstract class TitleConversion() : ValueConverter<Title, string>(c => c.Value, c => Title.FromString(c));