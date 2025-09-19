namespace MesEnterprise.Models
{
    public class SysFactory
    {
        public decimal FactoryId { get; set; }
        public string? FactoryCode { get; set; }
        public string? FactoryName { get; set; }
        public string? FactoryDesc { get; set; }
        public decimal? UpdateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string? Enabled { get; set; }
    }

    public class SysHtFactory
    {
        public decimal FactoryId { get; set; }
        public string? FactoryCode { get; set; }
        public string? FactoryName { get; set; }
        public string? FactoryDesc { get; set; }
        public decimal? UpdateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string? Enabled { get; set; }
    }
}
