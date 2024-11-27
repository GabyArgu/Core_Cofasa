using CoreContable.Entities.Views;

namespace CoreContable.Models.ResultSet;

public class RepositorioResultSet
{
    long RowNumber { get; set; }
    public string COD_CIA { get; set; }
    public string PERIODO { get; set; }
    public string TIPO_DOCTO { get; set; }
    public Select2ResultSet selTIPO_DOCTO { get; set; }
    public int NUM_POLIZA { get; set; }
    public string? NUM_REFERENCIA { get; set; }
    public DateTime? FECHA { get; set; }
    public string ANIO { get; set; }
    public string MES { get; set; }
    public string? CONCEPTO { get; set; }
    // public float? TOTAL_POLIZA { get; set; }
    public double? TOTAL_POLIZA { get; set; }
    public string STAT_POLIZA { get; set; }
    public Select2ResultSet selSTAT_POLIZA { get; set; }
    public DateTime? FECHA_CAMBIO { get; set; }
    public string? GRABACION_USUARIO { get; set; }
    public DateTime? GRABACION_FECHA { get; set; }
    public string? MODIFICACION_USUARIO { get; set; }
    public DateTime? MODIFICACION_FECHA { get; set; }
    public double? DiferenciaCargoAbono { get; set; }
    public string? NOMBRE_DOCTO { get; set; }

    public static RepositorioResultSet ViewToResultSet(RepositorioView entity)
    {
        return new RepositorioResultSet
        {
            RowNumber = entity.RowNum,
            COD_CIA = entity.COD_CIA,
            PERIODO = $"{entity.PERIODO}",
            TIPO_DOCTO = entity.TIPO_DOCTO,
            NUM_POLIZA = entity.NUM_POLIZA,
            NUM_REFERENCIA = entity.NUM_REFERENCIA,
            FECHA = entity.FECHA,
            ANIO = $"{entity.ANIO}",
            MES = $"{entity.MES}",
            CONCEPTO = entity.CONCEPTO,
            TOTAL_POLIZA = entity.TOTAL_POLIZA,
            STAT_POLIZA = entity.STAT_POLIZA,
            FECHA_CAMBIO = entity.FECHA_CAMBIO,
            // GRABACION_USUARIO = entity.GRABACION_USUARIO,
            GRABACION_FECHA = entity.GRABACION_FECHA,
            // MODIFICACION_USUARIO = entity.MODIFICACION_USUARIO,
            // MODIFICACION_FECHA = entity.MODIFICACION_FECHA,
            DiferenciaCargoAbono = entity.DiferenciaCargoAbono,
            NOMBRE_DOCTO = entity.NOMBRE_DOCTO
        };
    }
}