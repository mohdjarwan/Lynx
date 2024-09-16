namespace Lynx.Core.Entities
{
    public class Common
    {
        public string? CreatedBy { get; set; }

        public string LastModifiedBy { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime LastModifiedDate { get; set; } = DateTime.Now;
    }
}
