using Cyrus.Core.Domain.Toolkits.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cyrus.Infrastructure.Data.Sql.Commands.ValueConversions;

public class LegalNationalIdConversion()
    : ValueConverter<LegalNationalId, string>(c => c.Value, c => LegalNationalId.FromString(c));