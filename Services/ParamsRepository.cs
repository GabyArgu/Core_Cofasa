using System.Data;
using System.Data.Common;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CoreContable.Services;

enum ParamType 
{
    Conta,
    Period,
    CieCierre
}

public interface IParamsRepository
{
    Task<ContaParamsResultSet?> GetContaParamsByCia(string codCia);

    Task<bool> SaveContaParams(ContaParamsDto data);

    Task<bool> UpdateContaParams(ContaParamsDto data);
    
    Task<DmgPeriodResultSet?> GetPeriodParamsByCiaAndPeriod(string codCia, int period);

    Task<bool> SavePeriodParams(DmgPeriodDto data);

    Task<bool> UpdatePeriodParams(DmgPeriodDto data);

    Task<bool> ChangeStatusDmgCieCierre(DmgCieCierreDto data);
}

public class ParamsRepository(
    DbContext dbContext,
    ILogger<ParamsRepository> logger
) : IParamsRepository
{
    public Task<ContaParamsResultSet?> GetContaParamsByCia(string codCia) => dbContext.DmgParam
        .FromSql($"SELECT * FROM CONTABLE.ConsultarDMGParam({codCia})")
        .Select(param => new ContaParamsResultSet(param))
        .FirstOrDefaultAsync();

    private DbCommand AddParamsForContaParamsUpdateSave(DbCommand command, ContaParamsDto data)
    {
        command.Parameters.Add(new SqlParameter("@COD_CIA", SqlDbType.VarChar) { Value = data.COD_CIA==null ? DBNull.Value : data.COD_CIA });
        command.Parameters.Add(new SqlParameter("@DT1", SqlDbType.Int) { Value = data.DT1==null ? DBNull.Value : data.DT1 });
        command.Parameters.Add(new SqlParameter("@DT2", SqlDbType.Int) { Value = data.DT2==null ? DBNull.Value : data.DT2 });
        command.Parameters.Add(new SqlParameter("@DT3", SqlDbType.Int) { Value = data.DT3==null ? DBNull.Value : data.DT3 });
        command.Parameters.Add(new SqlParameter("@DT4", SqlDbType.Int) { Value = data.DT4==null ? DBNull.Value : data.DT4 });
        command.Parameters.Add(new SqlParameter("@DT5", SqlDbType.Int) { Value = data.DT5==null ? DBNull.Value : data.DT5 });
        command.Parameters.Add(new SqlParameter("@DT6", SqlDbType.Int) { Value = data.DT6==null ? DBNull.Value : data.DT6 });
        command.Parameters.Add(new SqlParameter("@CXC1", SqlDbType.Int) { Value = data.CXC1==null ? DBNull.Value : data.CXC1 });
        command.Parameters.Add(new SqlParameter("@CXC2", SqlDbType.Int) { Value = data.CXC2==null ? DBNull.Value : data.CXC2 });
        command.Parameters.Add(new SqlParameter("@CXC3", SqlDbType.Int) { Value = data.CXC3==null ? DBNull.Value : data.CXC3 });
        command.Parameters.Add(new SqlParameter("@CXC4", SqlDbType.Int) { Value = data.CXC4==null ? DBNull.Value : data.CXC4 });
        command.Parameters.Add(new SqlParameter("@CXC5", SqlDbType.Int) { Value = data.CXC5==null ? DBNull.Value : data.CXC5 });
        command.Parameters.Add(new SqlParameter("@CXC6", SqlDbType.Int) { Value = data.CXC6==null ? DBNull.Value : data.CXC6 });
        command.Parameters.Add(new SqlParameter("@CXP1", SqlDbType.Int) { Value = data.CXP1==null ? DBNull.Value : data.CXP1 });
        command.Parameters.Add(new SqlParameter("@CXP2", SqlDbType.Int) { Value = data.CXP2==null ? DBNull.Value : data.CXP2 });
        command.Parameters.Add(new SqlParameter("@CXP3", SqlDbType.Int) { Value = data.CXP3==null ? DBNull.Value : data.CXP3 });
        command.Parameters.Add(new SqlParameter("@CXP4", SqlDbType.Int) { Value = data.CXP4==null ? DBNull.Value : data.CXP4 });
        command.Parameters.Add(new SqlParameter("@CXP5", SqlDbType.Int) { Value = data.CXP5==null ? DBNull.Value : data.CXP5 });
        command.Parameters.Add(new SqlParameter("@CXP6", SqlDbType.Int) { Value = data.CXP6==null ? DBNull.Value : data.CXP6 });
        command.Parameters.Add(new SqlParameter("@EPF1", SqlDbType.Int) { Value = data.EPF1==null ? DBNull.Value : data.EPF1 });
        command.Parameters.Add(new SqlParameter("@EPF2", SqlDbType.Int) { Value = data.EPF2==null ? DBNull.Value : data.EPF2 });
        command.Parameters.Add(new SqlParameter("@EPF3", SqlDbType.Int) { Value = data.EPF3==null ? DBNull.Value : data.EPF3 });
        command.Parameters.Add(new SqlParameter("@EPF4", SqlDbType.Int) { Value = data.EPF4==null ? DBNull.Value : data.EPF4 });
        command.Parameters.Add(new SqlParameter("@EPF5", SqlDbType.Int) { Value = data.EPF5==null ? DBNull.Value : data.EPF5 });
        command.Parameters.Add(new SqlParameter("@EPF6", SqlDbType.Int) { Value = data.EPF6==null ? DBNull.Value : data.EPF6 });
        command.Parameters.Add(new SqlParameter("@IVA1", SqlDbType.Int) { Value = data.IVA1==null ? DBNull.Value : data.IVA1 });
        command.Parameters.Add(new SqlParameter("@IVA2", SqlDbType.Int) { Value = data.IVA2==null ? DBNull.Value : data.IVA2 });
        command.Parameters.Add(new SqlParameter("@IVA3", SqlDbType.Int) { Value = data.IVA3==null ? DBNull.Value : data.IVA3 });
        command.Parameters.Add(new SqlParameter("@IVA4", SqlDbType.Int) { Value = data.IVA4==null ? DBNull.Value : data.IVA4 });
        command.Parameters.Add(new SqlParameter("@IVA5", SqlDbType.Int) { Value = data.IVA5==null ? DBNull.Value : data.IVA5 });
        command.Parameters.Add(new SqlParameter("@IVA6", SqlDbType.Int) { Value = data.IVA6==null ? DBNull.Value : data.IVA6 });
        command.Parameters.Add(new SqlParameter("@CXPE1", SqlDbType.Int) { Value = data.CXPE1==null ? DBNull.Value : data.CXPE1 });
        command.Parameters.Add(new SqlParameter("@CXPE2", SqlDbType.Int) { Value = data.CXPE2==null ? DBNull.Value : data.CXPE2 });
        command.Parameters.Add(new SqlParameter("@CXPE3", SqlDbType.Int) { Value = data.CXPE3==null ? DBNull.Value : data.CXPE3 });
        command.Parameters.Add(new SqlParameter("@CXPE4", SqlDbType.Int) { Value = data.CXPE4==null ? DBNull.Value : data.CXPE4 });
        command.Parameters.Add(new SqlParameter("@CXPE5", SqlDbType.Int) { Value = data.CXPE5==null ? DBNull.Value : data.CXPE5 });
        command.Parameters.Add(new SqlParameter("@CXPE6", SqlDbType.Int) { Value = data.CXPE6==null ? DBNull.Value : data.CXPE6 });
        command.Parameters.Add(new SqlParameter("@DIF1", SqlDbType.Int) { Value = data.DIF1==null ? DBNull.Value : data.DIF1 });
        command.Parameters.Add(new SqlParameter("@DIF2", SqlDbType.Int) { Value = data.DIF2==null ? DBNull.Value : data.DIF2 });
        command.Parameters.Add(new SqlParameter("@DIF3", SqlDbType.Int) { Value = data.DIF3==null ? DBNull.Value : data.DIF3 });
        command.Parameters.Add(new SqlParameter("@DIF4", SqlDbType.Int) { Value = data.DIF4==null ? DBNull.Value : data.DIF4 });
        command.Parameters.Add(new SqlParameter("@DIF5", SqlDbType.Int) { Value = data.DIF5==null ? DBNull.Value : data.DIF5 });
        command.Parameters.Add(new SqlParameter("@DIF6", SqlDbType.Int) { Value = data.DIF6==null ? DBNull.Value : data.DIF6 });
        command.Parameters.Add(new SqlParameter("@DIA1", SqlDbType.Int) { Value = data.DIA1==null ? DBNull.Value : data.DIA1 });
        command.Parameters.Add(new SqlParameter("@DIA2", SqlDbType.Int) { Value = data.DIA2==null ? DBNull.Value : data.DIA2 });
        command.Parameters.Add(new SqlParameter("@DIA3", SqlDbType.Int) { Value = data.DIA3==null ? DBNull.Value : data.DIA3 });
        command.Parameters.Add(new SqlParameter("@DIA4", SqlDbType.Int) { Value = data.DIA4==null ? DBNull.Value : data.DIA4 });
        command.Parameters.Add(new SqlParameter("@DIA5", SqlDbType.Int) { Value = data.DIA5==null ? DBNull.Value : data.DIA5 });
        command.Parameters.Add(new SqlParameter("@DIA6", SqlDbType.Int) { Value = data.DIA6==null ? DBNull.Value : data.DIA6 });
        command.Parameters.Add(new SqlParameter("@OTR1", SqlDbType.Int) { Value = data.OTR1==null ? DBNull.Value : data.OTR1 });
        command.Parameters.Add(new SqlParameter("@OTR2", SqlDbType.Int) { Value = data.OTR2==null ? DBNull.Value : data.OTR2 });
        command.Parameters.Add(new SqlParameter("@OTR3", SqlDbType.Int) { Value = data.OTR3==null ? DBNull.Value : data.OTR3 });
        command.Parameters.Add(new SqlParameter("@OTR4", SqlDbType.Int) { Value = data.OTR4==null ? DBNull.Value : data.OTR4 });
        command.Parameters.Add(new SqlParameter("@OTR5", SqlDbType.Int) { Value = data.OTR5==null ? DBNull.Value : data.OTR5 });
        command.Parameters.Add(new SqlParameter("@OTR6", SqlDbType.Int) { Value = data.OTR6==null ? DBNull.Value : data.OTR6 });
        command.Parameters.Add(new SqlParameter("@IVAD1", SqlDbType.Int) { Value = data.IVAD1==null ? DBNull.Value : data.IVAD1 });
        command.Parameters.Add(new SqlParameter("@IVAD2", SqlDbType.Int) { Value = data.IVAD2==null ? DBNull.Value : data.IVAD2 });
        command.Parameters.Add(new SqlParameter("@IVAD3", SqlDbType.Int) { Value = data.IVAD3==null ? DBNull.Value : data.IVAD3 });
        command.Parameters.Add(new SqlParameter("@IVAD4", SqlDbType.Int) { Value = data.IVAD4==null ? DBNull.Value : data.IVAD4 });
        command.Parameters.Add(new SqlParameter("@IVAD5", SqlDbType.Int) { Value = data.IVAD5==null ? DBNull.Value : data.IVAD5 });
        command.Parameters.Add(new SqlParameter("@IVAD6", SqlDbType.Int) { Value = data.IVAD6==null ? DBNull.Value : data.IVAD6 });
        command.Parameters.Add(new SqlParameter("@AGE1", SqlDbType.Int) { Value = data.AGE1==null ? DBNull.Value : data.AGE1 });
        command.Parameters.Add(new SqlParameter("@AGE2", SqlDbType.Int) { Value = data.AGE2==null ? DBNull.Value : data.AGE2 });
        command.Parameters.Add(new SqlParameter("@AGE3", SqlDbType.Int) { Value = data.AGE3==null ? DBNull.Value : data.AGE3 });
        command.Parameters.Add(new SqlParameter("@AGE4", SqlDbType.Int) { Value = data.AGE4==null ? DBNull.Value : data.AGE4 });
        command.Parameters.Add(new SqlParameter("@AGE5", SqlDbType.Int) { Value = data.AGE5==null ? DBNull.Value : data.AGE5 });
        command.Parameters.Add(new SqlParameter("@AGE6", SqlDbType.Int) { Value = data.AGE6==null ? DBNull.Value : data.AGE6 });
        command.Parameters.Add(new SqlParameter("@CXC_TC1", SqlDbType.Int) { Value = data.CXC_TC1==null ? DBNull.Value : data.CXC_TC1 });
        command.Parameters.Add(new SqlParameter("@CXC_TC2", SqlDbType.Int) { Value = data.CXC_TC2==null ? DBNull.Value : data.CXC_TC2 });
        command.Parameters.Add(new SqlParameter("@CXC_TC3", SqlDbType.Int) { Value = data.CXC_TC3==null ? DBNull.Value : data.CXC_TC3 });
        command.Parameters.Add(new SqlParameter("@CXC_TC4", SqlDbType.Int) { Value = data.CXC_TC4==null ? DBNull.Value : data.CXC_TC4 });
        command.Parameters.Add(new SqlParameter("@CXC_TC5", SqlDbType.Int) { Value = data.CXC_TC5==null ? DBNull.Value : data.CXC_TC5 });
        command.Parameters.Add(new SqlParameter("@CXC_TC6", SqlDbType.Int) { Value = data.CXC_TC6==null ? DBNull.Value : data.CXC_TC6 });
        command.Parameters.Add(new SqlParameter("@DSV1", SqlDbType.Int) { Value = data.DSV1==null ? DBNull.Value : data.DSV1 });
        command.Parameters.Add(new SqlParameter("@DSV2", SqlDbType.Int) { Value = data.DSV2==null ? DBNull.Value : data.DSV2 });
        command.Parameters.Add(new SqlParameter("@DSV3", SqlDbType.Int) { Value = data.DSV3==null ? DBNull.Value : data.DSV3 });
        command.Parameters.Add(new SqlParameter("@DSV4", SqlDbType.Int) { Value = data.DSV4==null ? DBNull.Value : data.DSV4 });
        command.Parameters.Add(new SqlParameter("@DSV5", SqlDbType.Int) { Value = data.DSV5==null ? DBNull.Value : data.DSV5 });
        command.Parameters.Add(new SqlParameter("@DSV6", SqlDbType.Int) { Value = data.DSV6==null ? DBNull.Value : data.DSV6 });
        
        command.Parameters.Add(new SqlParameter("@RIVA1", SqlDbType.Int) { Value = data.RIVA1==null ? DBNull.Value : data.RIVA1 });
        command.Parameters.Add(new SqlParameter("@RIVA2", SqlDbType.Int) { Value = data.RIVA2==null ? DBNull.Value : data.RIVA2 });
        command.Parameters.Add(new SqlParameter("@RIVA3", SqlDbType.Int) { Value = data.RIVA3==null ? DBNull.Value : data.RIVA3 });
        command.Parameters.Add(new SqlParameter("@RIVA4", SqlDbType.Int) { Value = data.RIVA4==null ? DBNull.Value : data.RIVA4 });
        command.Parameters.Add(new SqlParameter("@RIVA5", SqlDbType.Int) { Value = data.RIVA5==null ? DBNull.Value : data.RIVA5 });
        command.Parameters.Add(new SqlParameter("@RIVA6", SqlDbType.Int) { Value = data.RIVA6==null ? DBNull.Value : data.RIVA6 });

        command.Parameters.Add(new SqlParameter("@UsuarioCreacion", SqlDbType.VarChar) { Value = data.UsuarioCreacion==null ? DBNull.Value : data.UsuarioCreacion });
        command.Parameters.Add(new SqlParameter("@FechaCreacion", SqlDbType.DateTime) { Value = data.FechaCreacion==null ? DBNull.Value : data.FechaCreacion });
        command.Parameters.Add(new SqlParameter("@UsuarioModificacion", SqlDbType.VarChar) { Value = data.UsuarioModificacion==null ? DBNull.Value : data.UsuarioModificacion });
        command.Parameters.Add(new SqlParameter("@FechaModificacion", SqlDbType.DateTime) { Value = data.FechaModificacion==null ? DBNull.Value : data.FechaModificacion });

        return command;
    }

    private async Task<bool> DoSaveOrUpdate(dynamic data, string query, ParamType type, bool? isCreating = true)
    {
        var command = dbContext.Database.GetDbConnection().CreateCommand();

        try
        {
            command.CommandText = query;
            command.CommandType = CommandType.StoredProcedure;

            command = type switch
            {
                ParamType.Conta => AddParamsForContaParamsUpdateSave(command, data as ContaParamsDto),
                ParamType.Period => AddParamsForPeriodParamsUpdateSave(command, data as DmgPeriodDto, isCreating ?? true),
                ParamType.CieCierre => AddParamsForCieCierreUpdateStatus(command, data as DmgCieCierreDto),
                _ => command
            };

            if (command.Connection?.State != ConnectionState.Open) await dbContext.Database.OpenConnectionAsync();
            await command.ExecuteNonQueryAsync();
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();

            return true;
        }
        catch (Exception e)
        {
            if (command.Connection?.State == ConnectionState.Open)  await dbContext.Database.CloseConnectionAsync();
            logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                nameof(ParamsRepository), nameof(DoSaveOrUpdate));
            return false;
        }
    }

    public Task<bool> SaveContaParams(ContaParamsDto data) =>
        DoSaveOrUpdate(data, $"{CC.SCHEMA}.InsertarOActualizarDmgparam", ParamType.Conta);

    public Task<bool> UpdateContaParams(ContaParamsDto data) =>
        DoSaveOrUpdate(data, $"{CC.SCHEMA}.InsertarOActualizarDmgparam", ParamType.Conta);
    
    private DbCommand AddParamsForPeriodParamsUpdateSave(DbCommand command, DmgPeriodDto data, bool isCreating)
    {
        command.Parameters.Add(new SqlParameter("@COD_CIA", SqlDbType.VarChar) { Value = data.CodCia==null ? DBNull.Value : data.CodCia });
        command.Parameters.Add(new SqlParameter("@PERIODO", SqlDbType.Int) { Value = data.Period==null ? DBNull.Value : data.Period });
        command.Parameters.Add(new SqlParameter("@APERTURA", SqlDbType.DateTime) { Value = data.Opened==null ? DBNull.Value : data.Opened });
        command.Parameters.Add(new SqlParameter("@CIERRE", SqlDbType.DateTime) { Value = data.Closed==null ? DBNull.Value : data.Closed });
        command.Parameters.Add(new SqlParameter("@ESTADO", SqlDbType.VarChar) { Value = data.Status==null ? DBNull.Value : data.Status });
        command.Parameters.Add(new SqlParameter("@MES_INI", SqlDbType.Int) { Value = data.StartMonth==null ? DBNull.Value : data.StartMonth });
        command.Parameters.Add(new SqlParameter("@MES_FIN", SqlDbType.Int) { Value = data.FinishMonth==null ? DBNull.Value : data.FinishMonth });
        command.Parameters.Add(new SqlParameter("@UsuarioModificacion", SqlDbType.VarChar) { Value = data.UpdatedBy==null ? DBNull.Value : data.UpdatedBy });
        command.Parameters.Add(new SqlParameter("@FechaModificacion", SqlDbType.DateTime) { Value = data.UpdatedAt==null ? DBNull.Value : data.UpdatedAt });

        if (!isCreating) return command;
        command.Parameters.Add(new SqlParameter("@UsuarioCreacion", SqlDbType.VarChar) { Value = data.CreatedBy==null ? DBNull.Value : data.CreatedBy });
        command.Parameters.Add(new SqlParameter("@FechaCreacion", SqlDbType.DateTime) { Value = data.CreatedAt==null ? DBNull.Value : data.CreatedAt });

        return command;
    }

    public Task<DmgPeriodResultSet?> GetPeriodParamsByCiaAndPeriod(string codCia, int period) =>
        dbContext.DmgPeriod
            .FromSql($"SELECT * FROM CONTABLE.ConsultarPeriodo({codCia}, {period})")
            .Select(entity => new DmgPeriodResultSet
            {
                CodCia = entity.COD_CIA,
                Period = $"{entity.PERIODO}",
                Opened = entity.APERTURA,
                Closed = entity.CIERRE,
                Status = entity.ESTADO,
                StartMonth = $"{entity.MES_INI}",
                FinishMonth = $"{entity.MES_FIN}"
            })
            .FirstOrDefaultAsync();

    public Task<bool> SavePeriodParams(DmgPeriodDto data) =>
        DoSaveOrUpdate(data, $"{CC.SCHEMA}.InsertarPeriodo", ParamType.Period, isCreating: true);

    public Task<bool> UpdatePeriodParams(DmgPeriodDto data) =>
        DoSaveOrUpdate(data, $"{CC.SCHEMA}.ActualizarPeriodo", ParamType.Period, isCreating: false);

    private DbCommand AddParamsForCieCierreUpdateStatus(DbCommand command, DmgCieCierreDto data)
    {
        command.Parameters.Add(new SqlParameter("@CodigoCia", SqlDbType.VarChar) { Value = data.CIE_CODCIA==null ? DBNull.Value : data.CIE_CODCIA });
        command.Parameters.Add(new SqlParameter("@Codigo", SqlDbType.Int) { Value = data.CIE_CODIGO==null ? DBNull.Value : data.CIE_CODIGO });
        command.Parameters.Add(new SqlParameter("@NuevoEstado", SqlDbType.VarChar) { Value = data.CIE_ESTADO==null ? DBNull.Value : data.CIE_ESTADO });

        return command;
    }

    public Task<bool> ChangeStatusDmgCieCierre(DmgCieCierreDto data) =>
        DoSaveOrUpdate(data, $"{CC.SCHEMA}.ActualizarEstadoCierre", ParamType.CieCierre);
}