using System.Text;

namespace Zu.Firefox
{
    public class CacheEntry
    {
        public string Url { get; set; }
        public string IdEnhance { get; set; }
        public int DataSize { get; set; }
        public int FetchCount { get; set; }
        public string LastModifiedTime { get; set; }
        public string ExpirationTime { get; set; }
        public bool Pinned { get; set; }

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
            return Url;
        }
    }
}
