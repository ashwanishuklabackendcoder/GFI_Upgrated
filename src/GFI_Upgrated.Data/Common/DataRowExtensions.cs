using System.Data;

namespace GFI_Upgrated.Data.Store; // Keeping it in Store namespace for now to avoid breaking existing code if they didn't use using

public static class DataRowExtensions
{
    public static DateTime? SafeDateTime(this DataRow row, params string[] names)
    {
        foreach (var name in names)
        {
            if (row.Table.Columns.Contains(name) && row[name] != DBNull.Value && DateTime.TryParse(row[name].ToString(), out var value))
            {
                return value;
            }
        }
        return null;
    }

    public static long SafeLong(this DataRow row, params string[] names)
    {
        foreach (var name in names)
        {
            if (row.Table.Columns.Contains(name) && row[name] != DBNull.Value && long.TryParse(row[name].ToString(), out var value))
            {
                return value;
            }
        }
        return 0;
    }

    public static long? SafeLongNullable(this DataRow row, params string[] names)
    {
        foreach (var name in names)
        {
            if (row.Table.Columns.Contains(name) && row[name] != DBNull.Value && long.TryParse(row[name].ToString(), out var value))
            {
                return value;
            }
        }
        return null;
    }

    public static int SafeInt(this DataRow row, params string[] names)
    {
        foreach (var name in names)
        {
            if (row.Table.Columns.Contains(name) && row[name] != DBNull.Value && int.TryParse(row[name].ToString(), out var value))
            {
                return value;
            }
        }
        return 0;
    }

    public static double SafeDouble(this DataRow row, params string[] names)
    {
        foreach (var name in names)
        {
            if (row.Table.Columns.Contains(name) && row[name] != DBNull.Value && double.TryParse(row[name].ToString(), out var value))
            {
                return value;
            }
        }
        return 0;
    }

    public static bool SafeBool(this DataRow row, params string[] names)
    {
        foreach (var name in names)
        {
            if (row.Table.Columns.Contains(name) && row[name] != DBNull.Value && bool.TryParse(row[name].ToString(), out var value))
            {
                return value;
            }
            if (row.Table.Columns.Contains(name) && row[name] != DBNull.Value && int.TryParse(row[name].ToString(), out var numeric))
            {
                return numeric != 0;
            }
        }
        return false;
    }

    public static string SafeString(this DataRow row, params string[] names)
    {
        foreach (var name in names)
        {
            if (row.Table.Columns.Contains(name) && row[name] != DBNull.Value)
            {
                return Convert.ToString(row[name]) ?? string.Empty;
            }
        }
        return string.Empty;
    }
}
