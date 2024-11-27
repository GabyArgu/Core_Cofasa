using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreContable.Utils;

namespace CoreContable.Entities;

[Table(CC.REPOSITORYIMPORTLOG, Schema = CC.SCHEMA)]
public class RepositoryImportLog
{
    [Key]
    public int Id { get; set; }

    [MaxLength(3)]
    public required string CodCia { get; set; }

    public required int NumPoliza { get; set; }

    [MaxLength(2)]
    public required string TipoDocto { get; set; }
    
    // public required int NumRows { get; set; }
    
    [MaxLength(255)]
    public required string Description { get; set; }

    [MaxLength(50)]
    public required string UploadUser { get; set; }

    public DateTime? UploadAt { get; set; } = DateTime.Now;
}