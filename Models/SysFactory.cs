using System.ComponentModel.DataAnnotations;

namespace MesEnterprise.Models;

public class SysFactory
{
    [Key]
    public decimal FactoryId { get; set; }

    [Required]
    [StringLength(10)]
    public string? FactoryCode { get; set; }

    [Required]
    [StringLength(25)]
    public string? FactoryName { get; set; }

    [StringLength(50)]
    public string? FactoryDesc { get; set; }

    public decimal? UpdateUserId { get; set; }

    public DateTime? UpdateTime { get; set; }

    [StringLength(1)]
    public string? Enabled { get; set; }
}

public class SysHtFactory
{
    public decimal FactoryId { get; set; }

    [Required]
    [StringLength(10)]
    public string? FactoryCode { get; set; }

    [Required]
    [StringLength(25)]
    public string? FactoryName { get; set; }

    [StringLength(50)]
    public string? FactoryDesc { get; set; }

    public decimal? UpdateUserId { get; set; }

    public DateTime? UpdateTime { get; set; }

    [StringLength(1)]
    public string? Enabled { get; set; }
}
