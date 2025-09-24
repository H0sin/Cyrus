using Cyrus.Core.Domain.Toolkits.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cyrus.Infrastructure.Data.Sql.Commands.ValueConversions;

public class NationalCodeConversion()
    : ValueConverter<NationalCode, string>(c => c.Value, c => NationalCode.FromString(c));