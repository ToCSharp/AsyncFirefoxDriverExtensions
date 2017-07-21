using System.Text;

namespace Zu.Firefox
{
    public class CacheInfo
    {
        public int EntryCount { get; set; }
        public int Consumption { get; set; }
        public int Capacity { get; set; }
        public string DiskDirectory { get; set; }
        public CacheEntry[] entries { get; set; }

        public string Error { get; set; }


        public string GetInfo()
        {
            var sb = new StringBuilder();
            foreach (var prop in GetType().GetProperties())
            {
                var pv = prop.GetValue(this);
                if (pv != null)
                {
                    sb.AppendLine($"{prop.Name} = {pv}");
                }
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return DiskDirectory;
        }
    }
}
