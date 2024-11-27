using CoreContable.Entities.FuntionResult;

namespace CoreContable.Models.ResultSet;

public class DetRepositorioResultSet
{
    static DetRepositorioResultSet()
    {
    }

    public string COD_CIA { get; set; }
    public int PERIODO { get; set; }
    public string TIPO_DOCTO { get; set; }
    public int NUM_POLIZA { get; set; }
    public string Desc_CCosto { get; set; }
    public int CORRELAT { get; set; }
    public int CTA_1 { get; set; }
    public int CTA_2 { get; set; }
    public int CTA_3 { get; set; }
    public int CTA_4 { get; set; }
    public int CTA_5 { get; set; }
    public int CTA_6 { get; set; }
    public string Desc_CContable { get; set; }
    public string? CONCEPTO { get; set; }
    public double? CARGO { get; set; }
    public double? ABONO { get; set; }
    
    // Select2
    public Select2ResultSet? selCentroCosto { get; set; }
    public Select2ResultSet? selCentroCuenta { get; set; }
}

// public static class DetRepositorioResultSetExtensions
// {
//     public static DetRepositorioResultSet ToResultSet(
//         this DetRepositorioFromFuncForDt entity)
//     {
//         resultSet.COD_CIA = entity.COD_CIA;
//         resultSet.PERIODO = entity.PERIODO;
//         resultSet.TIPO_DOCTO = entity.TIPO_DOCTO;
//         resultSet.NUM_POLIZA = entity.NUM_POLIZA;
//         resultSet.Desc_CCosto = entity.Desc_CCosto;
//         resultSet.selCentroCosto = new Select2ResultSet
//         {
//             id = entity.CENTRO_COSTO,
//             text = entity.Desc_CCosto
//         };
//         resultSet.selCentroCuenta = new Select2ResultSet
//         {
//             id =
//                 $"{entity.COD_CIA}|{entity.CENTRO_COSTO}|{entity.CTA_1}|{entity.CTA_2}|{entity.CTA_3}|{entity.CTA_4}|{entity.CTA_5}|{entity.CTA_6}",
//             text = $"{entity.CTA_1}{entity.CTA_2}{entity.CTA_3}{entity.CTA_4}{entity.CTA_5}{entity.CTA_6}"
//         };
//         resultSet.CORRELAT = entity.CORRELAT;
//         resultSet.CTA_1 = entity.CTA_1;
//         resultSet.CTA_2 = entity.CTA_2;
//         resultSet.CTA_3 = entity.CTA_3;
//         resultSet.CTA_4 = entity.CTA_4;
//         resultSet.CTA_5 = entity.CTA_5;
//         resultSet.CTA_6 = entity.CTA_6;
//         resultSet.Desc_CContable = entity.Desc_CContable;
//         resultSet.CONCEPTO = entity.CONCEPTO;
//         resultSet.CARGO = entity.CARGO;
//         resultSet.ABONO = entity.ABONO;
//         return resultSet;
//     }
// }