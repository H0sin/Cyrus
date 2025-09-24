using System.Data;
using System.Globalization;
using System.Reflection;
using ClosedXML.Excel;
using Cyrus.Utilities.Abstractions;

namespace Cyrus.Utilities.Excel;

/// <summary>
/// Excel serializer based on ClosedXML: creates worksheets from lists/objects
/// and parses workbooks back into DataTable or strongly-typed lists.
/// </summary>
public sealed class ClosedXmlExcelSerializer : IExcelSerializer
{
    public byte[] ListToExcelByteArray<T>(List<T> list, string sheetName = "Result")
    {
        using var workbook = new XLWorkbook();
        var table = ToDataTable(list);
        var ws = workbook.Worksheets.Add(table, string.IsNullOrWhiteSpace(sheetName) ? "Result" : sheetName);
        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    public DataTable ExcelToDataTable(byte[] bytes)
    {
        using var ms = new MemoryStream(bytes, writable: false);
        using var workbook = new XLWorkbook(ms);
        var ws = workbook.Worksheets.Worksheet(1);
        var dt = new DataTable(ws.Name);

        var firstRowUsed = ws.FirstRowUsed();
        if (firstRowUsed is null) return dt;

        var headerRow = firstRowUsed.RowUsed();
        foreach (var cell in headerRow.CellsUsed())
            dt.Columns.Add(new DataColumn(cell.GetString()));

        // Iterate data rows from the row below header up to the last used row in the worksheet
        var lastRowUsed = ws.LastRowUsed();
        if (lastRowUsed is null) return dt;

        for (int rowNum = headerRow.RowNumber() + 1; rowNum <= lastRowUsed.RowNumber(); rowNum++)
        {
            var row = ws.Row(rowNum);
            var dataRow = dt.NewRow();
            int i = 0;
            foreach (var cell in row.Cells(1, dt.Columns.Count))
            {
                object value = cell.IsEmpty() ? DBNull.Value : cell.DataType switch
                {
                    XLDataType.Text => cell.GetString(),
                    XLDataType.Number => cell.GetDouble(),
                    XLDataType.DateTime => cell.GetDateTime(),
                    XLDataType.Boolean => cell.GetBoolean(),
                    XLDataType.TimeSpan => cell.GetTimeSpan(),
                    _ => cell.GetString()
                };
                dataRow[i++] = value;
            }
            dt.Rows.Add(dataRow);
        }
        return dt;
    }

    public List<T> ExcelToList<T>(byte[] bytes)
    {
        var dt = ExcelToDataTable(bytes);
        return FromDataTable<T>(dt);
    }

    // Helpers
    private static DataTable ToDataTable<T>(IEnumerable<T> list)
    {
        var dt = new DataTable(typeof(T).Name);
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                              .Where(p => p.CanRead)
                              .ToArray();
        foreach (var p in props)
        {
            var colType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
            dt.Columns.Add(p.Name, colType);
        }
        foreach (var item in list)
        {
            var values = new object?[props.Length];
            for (int i = 0; i < props.Length; i++)
            {
                values[i] = props[i].GetValue(item) ?? DBNull.Value;
            }
            dt.Rows.Add(values);
        }
        return dt;
    }

    private static List<T> FromDataTable<T>(DataTable table)
    {
        var result = new List<T>(table.Rows.Count);
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                              .Where(p => p.CanWrite)
                              .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

        foreach (DataRow row in table.Rows)
        {
            var obj = Activator.CreateInstance<T>();
            foreach (DataColumn col in table.Columns)
            {
                if (!props.TryGetValue(col.ColumnName, out var prop)) continue;
                var val = row[col];
                if (val is DBNull) continue;
                try
                {
                    object? converted = ConvertTo(val, prop.PropertyType);
                    prop.SetValue(obj, converted);
                }
                catch
                {
                    // ignore individual conversion errors; continue mapping
                }
            }
            result.Add(obj);
        }
        return result;
    }

    private static object? ConvertTo(object value, Type targetType)
    {
        if (value is null) return null;
        var t = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (t.IsEnum)
        {
            if (value is string s) return Enum.Parse(t, s, ignoreCase: true);
            return Enum.ToObject(t, System.Convert.ChangeType(value, Enum.GetUnderlyingType(t), CultureInfo.InvariantCulture)!);
        }
        if (t == typeof(Guid))
        {
            return value is Guid g ? g : Guid.Parse(value.ToString()!);
        }
        if (t == typeof(DateOnly))
        {
            if (value is DateOnly d) return d;
            if (value is DateTime dt) return DateOnly.FromDateTime(dt);
            return DateOnly.Parse(value.ToString()!, CultureInfo.InvariantCulture);
        }
        if (t == typeof(TimeOnly))
        {
            if (value is TimeOnly to) return to;
            if (value is TimeSpan ts) return TimeOnly.FromTimeSpan(ts);
            return TimeOnly.Parse(value.ToString()!, CultureInfo.InvariantCulture);
        }
        if (t == typeof(bool) && value is string sb)
        {
            if (bool.TryParse(sb, out var b)) return b;
            if (int.TryParse(sb, out var bi)) return bi != 0;
        }
        return System.Convert.ChangeType(value, t, CultureInfo.InvariantCulture);
    }
}
