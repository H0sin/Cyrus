using Cyrus.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cyrus.Infrastructure.Data.Sql.Commands.ValueConversions
{
    public class BusinessIdConversion() : ValueConverter<BusinessId, Guid>(c => c.Value, c => BusinessId.FromGuid(c));
}
