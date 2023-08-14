using System.Globalization;
using CsvHelper;
using UserApi.Models;

namespace UserApi.Libraries;

public static class ReadCsvFile
{
    public static List<User> Read(string path)
    {
        using var streamReader = new StreamReader(path);
        using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
        List<User> records = csvReader.GetRecords<User>().ToList();
        return records;
    }
}