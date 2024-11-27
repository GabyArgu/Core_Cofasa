using CoreContable.Entities;

namespace CoreContable.Models.ResultSet;

public class DmgDoctosResultSet
{
    public DmgDoctosResultSet(DmgDoctos entity)
    {
        COD_CIA = entity.COD_CIA;
        TIPO_DOCTO = entity.TIPO_DOCTO;
        CONTADOR_POLIZA = entity.CONTADOR_POLIZA;
        DESCRIP_TIPO = entity.DESCRIP_TIPO;
        PROCESO = entity.PROCESO;
        POLIZA_MANUAL = entity.POLIZA_MANUAL;
        UsuarioCreacion = entity.UsuarioCreacion;
        FechaCreacion = entity.FechaCreacion;
        UsuarioModificacion = entity.UsuarioModificacion;
        FechaModificacion = entity.FechaModificacion;
    }

    public DmgDoctosResultSet()
    {
    }

    public string? COD_CIA { get; set; }
    public string? TIPO_DOCTO { get; set; }
    public int? CONTADOR_POLIZA { get; set; }
    public string? DESCRIP_TIPO { get; set; }
    public string? PROCESO { get; set; }
    public string? POLIZA_MANUAL { get; set; }
    public string? UsuarioCreacion { get; set; }
    public DateTime? FechaCreacion { get; set; }
    public string? UsuarioModificacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
}