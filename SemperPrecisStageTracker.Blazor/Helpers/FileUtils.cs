using Microsoft.JSInterop;
using System.Text.RegularExpressions;

namespace SemperPrecisStageTracker.Blazor.Helpers
{
    public static class FileUtils
    {
        public static void SaveAs(this IJSRuntime js, string filename, byte[] data)
        {
            ((IJSInProcessRuntime)js).Invoke<object>(
                "customFunctions.saveAsFile",
                filename,
                Convert.ToBase64String(data));
        }
    }

    public class CSVHelper : List<string[]>
    {
        protected string csv = string.Empty;
        protected string separator = ",";

        public CSVHelper(string csv, string separator = ",")
        {
            this.csv = csv;
            this.separator = separator;

            foreach (string line in Regex.Split(csv, System.Environment.NewLine).Skip(1).ToList().Where(s => !string.IsNullOrEmpty(s)))
            {
                string[] values = Regex.Split(line, separator);

                for (int i = 0; i < values.Length; i++)
                {
                    //Trim values
                    values[i] = values[i].Trim();
                }

                this.Add(values);
            }
        }
    }
}