using Rotativa.AspNetCore.Options;
using CoreContable.Services;

namespace CoreContable.Utils;

public abstract class CC
{
    // GENERA VARIABLES.
    public const string ADA = "ADA";
    public const string JOJO = "JOJO";
    public const string DIO = "DIO";

    // NOMBRES DE RUTAS
    public const string SecurityControllerName = "Security";
    public const string LoginActionName = "Login";
    public const string LogOutActionName = "LogOut";
    public const string HomeControllerName = "Home";
    public const string DashboardActionName = "Index";
    public const string ErrorActionName = "Error";

    // SESSION VARIABLES KEYS
    public const string CiaCodeKey = "CiaCode";
    public const string CiaNameKey = "CiaName";
    public const string UserNameKey = "UserName";
    public const string UserIdKey = "UserId";
    public const string FullNameKey = "FullName";
    public const string UserPermissionsKey = "UserPermissions";

    // NOMBRES DE TABLAS Y SCHEMA.

    public const string SCHEMA = "CONTABLE";
    // public const string SCHEMA = "dbo";

    public const string AC_MON_MONEDA = "ac_mon_moneda";
    public const string CENTRO_COSTO = "centro_costo";
    public const string CENTRO_CUENTA = "centro_cuenta";
    public const string CIAS = "cias";
    public const string DET_REPOSITORIO = "det_repositorio";
    public const string DMG_CIE_CIERRE = "dmg_cie_cierre";
    public const string DMGCUENTAS = "dmgcuentas";
    public const string DMGDOCTOS = "dmgdoctos";
    public const string DMGPARAM = "dmgparam";
    public const string DMGPERIODO = "dmgperiodo";
    public const string REPOSITORIO = "repositorio";
    public const string PERMISSION = "Permission";
    public const string ROLE = "Role";
    public const string ROLEPERMISSION = "RolePermission";
    public const string USERAPP = "UserApp";
    public const string USERCIA = "UserCia";
    public const string USERROLE = "UserRole";
    public const string DMGPOLIZA = "dmgpoliza";
    public const string DMGDETALLE = "dmgdetalle";
    public const string DMGNUMERA = "dmgnumera";
    public const string REPOSITORYIMPORTLOG = "repository_import_log";

    public const string RoleAdmin = "admin";
    public const string RoleReports = "reports";

    // ALIAS DE PERMISOS PARA ROLES.

    // FIRST LEVEL PERMISSIONS
    public const string FIST_LEVEL_PERMISSION_DASHBOARD = "DASHBOARD";
    public const string FIST_LEVEL_PERMISSION_SECURITY_MODULE = "SECURITY_MODULE";
    public const string FIST_LEVEL_PERMISSION_CONTABILITY_MODULE = "CONTABILITY_MODULE";
    public const string FIST_LEVEL_PERMISSION_REPORTS = "REPORTS";

    // SECOND LEVEL PERMISSIONS

    // SECURITY_MODULE
    public const string SECOND_LEVEL_PERMISSION_ADMIN_USERS = "ADMIN_USERS";
    public const string THIRD_LEVEL_PERMISSION_USERS_CAN_ADD = "USERS_CAN_ADD";
    public const string THIRD_LEVEL_PERMISSION_USERS_CAN_UPDATE = "USERS_CAN_UPDATE";
    public const string THIRD_LEVEL_PERMISSION_USERS_CAN_DELETE = "USERS_CAN_DELETE";
    public const string THIRD_LEVEL_PERMISSION_USERS_CAN_COMPANY = "USERS_CAN_COMPANY";

    public const string SECOND_LEVEL_PERMISSION_ADMIN_ROLES = "ADMIN_ROLES";
    public const string THIRD_LEVEL_PERMISSION_ROLES_CAN_ADD = "ROLES_CAN_ADD";
    public const string THIRD_LEVEL_PERMISSION_ROLES_CAN_UPDATE = "ROLES_CAN_UPDATE";
    public const string THIRD_LEVEL_PERMISSION_ROLES_CAN_DELETE = "ROLES_CAN_DELETE";
    public const string THIRD_LEVEL_PERMISSION_ROLES_PER_COMPANY = "ROLES_PER_COMPANY";

    // CONTABILITY_MODULE
    public const string SECOND_LEVEL_PERMISSION_ADMIN_CIAS = "ADMIN_CIAS";
    public const string THIRD_LEVEL_PERMISSION_CIAS_CAN_ADD = "CIAS_CAN_ADD";
    public const string THIRD_LEVEL_PERMISSION_CIAS_CAN_UPDATE = "CIAS_CAN_UPDATE";
    public const string THIRD_LEVEL_PERMISSION_CIAS_CAN_DELETE = "CIAS_CAN_DELETE";
    public const string THIRD_LEVEL_PERMISSION_CIAS_CAN_COPY = "CIAS_CAN_COPY";

    public const string SECOND_LEVEL_PERMISSION_ADMIN_PARAMS_CONTA = "PARAMS_CONTA";
    public const string THIRD_LEVEL_PERMISSION_PARAMS_CONTA_CAN_ADD = "PARAMS_CONTA_CAN_ADD";
    public const string THIRD_LEVEL_PERMISSION_PARAMS_CONTA_CAN_UPDATE = "PARAMS_CONTA_CAN_UPDATE";

    public const string SECOND_LEVEL_PERMISSION_ADMIN_PARAMS_PERIOD = "PARAMS_PERIOD";
    public const string THIRD_LEVEL_PERMISSION_PARAMS_PERIOD_CAN_ADD = "PARAMS_PERIOD_CAN_ADD";
    public const string THIRD_LEVEL_PERMISSION_PARAMS_PERIOD_CAN_UPDATE = "PARAMS_PERIOD_CAN_UPDATE";
    public const string THIRD_LEVEL_PERMISSION_PARAMS_PERIOD_CAN_CHANGE_STATUS = "PARAMS_PERIOD_CAN_CHANGE_STATUS";

    public const string SECOND_LEVEL_PERMISSION_ADMIN_DMGDOCTOS = "DMGDOCTOS";
    public const string THIRD_LEVEL_PERMISSION_DMGDOCTOS_CAN_ADD = "DMGDOCTOS_CAN_ADD";
    public const string THIRD_LEVEL_PERMISSION_DMGDOCTOS_CAN_UPDATE = "DMGDOCTOS_CAN_UPDATE";

    public const string SECOND_LEVEL_PERMISSION_ADMIN_CURRENCY = "CURRENCY";
    public const string THIRD_LEVEL_PERMISSION_CURRENCY_CAN_ADD = "CURRENCY_CAN_ADD";
    public const string THIRD_LEVEL_PERMISSION_CURRENCY_CAN_UPDATE = "CURRENCY_CAN_UPDATE";

    public const string SECOND_LEVEL_PERMISSION_ADMIN_DMGCUENTAS = "DMGCUENTAS";
    public const string THIRD_LEVEL_PERMISSION_DMGCUENTAS_CAN_ADD = "DMGCUENTAS_CAN_ADD";
    public const string THIRD_LEVEL_PERMISSION_DMGCUENTAS_CAN_UPDATE = "DMGCUENTAS_CAN_UPDATE";

    public const string SECOND_LEVEL_PERMISSION_ADMIN_CENTROCOSTO = "CENTROCOSTO";
    public const string THIRD_LEVEL_PERMISSION_CENTROCOSTO_CAN_ADD = "CENTROCOSTO_CAN_ADD";
    public const string THIRD_LEVEL_PERMISSION_CENTROCOSTO_CAN_UPDATE = "CENTROCOSTO_CAN_UPDATE";
    // public const string THIRD_LEVEL_PERMISSION_CENTROCOSTO_CAN_ADMIN_ACCOUNTS = "CENTROCOSTO_CAN_ADMIN_ACCOUNTS";
    public const string THIRD_LEVEL_PERMISSION_CENTROCOSTO_CAN_COPY = "CENTROCOSTO_CAN_COPY";


    // CENTRO_CUENTA MODULE
    public const string THIRD_LEVEL_PERMISSION_ADMIN_CENTRO_CUENTA = "CENTRO_CUENTA";
    public const string THIRD_LEVEL_PERMISSION_CENTRO_CUENTA_CAN_ADD = "CENTRO_CUENTA_CAN_ADD";
    public const string THIRD_LEVEL_PERMISSION_CENTRO_CUENTA_CAN_UPDATE = "CENTRO_CUENTA_CAN_UPDATE";
    public const string THIRD_LEVEL_PERMISSION_CENTRO_CUENTA_CAN_EDIT = "CENTRO_CUENTA_CAN_EDIT";
    public const string THIRD_LEVEL_PERMISSION_CENTRO_CUENTA_CAN_DELETE = "CENTRO_CUENTA_CAN_DELETE";

    public const string SECOND_LEVEL_PERMISSION_ADMIN_REPOSITORIO = "REPOSITORIO";
    public const string THIRD_LEVEL_PERMISSION_REPOSITORIO_CAN_ADD = "REPOSITORIO_CAN_ADD";
    public const string THIRD_LEVEL_PERMISSION_REPOSITORIO_CAN_UPDATE = "REPOSITORIO_CAN_UPDATE";
    public const string THIRD_LEVEL_PERMISSION_REPOSITORIO_CAN_DELETE = "REPOSITORIO_CAN_DELETE";
    public const string THIRD_LEVEL_PERMISSION_REPOSITORIO_CAN_CAPITALIZE = "REPOSITORIO_CAN_CAPITALIZE";
    public const string THIRD_LEVEL_PERMISSION_REPOSITORIO_CAN_PRINT = "REPOSITORIO_CAN_PRINT";
    public const string THIRD_LEVEL_PERMISSION_REPOSITORIO_CAN_IMPORT_FROM_EXCEL = "REPOSITORIO_CAN_IMPORT_FROM_EXCEL";

    public const string THIRD_LEVEL_PERMISSION_DET_REPOSITORIO = "DET_REPOSITORIO";
    public const string THIRD_LEVEL_PERMISSION_DET_REPOSITORIO_CAN_ADD = "DET_REPOSITORIO_CAN_ADD";
    public const string THIRD_LEVEL_PERMISSION_DET_REPOSITORIO_CAN_UPDATE = "DET_REPOSITORIO_CAN_UPDATE";

    public const string SECOND_LEVEL_PERMISSION_ADMIN_DMGPOLIZA = "DMGPOLIZA";
    public const string THIRD_LEVEL_PERMISSION_DMGPOLIZA_CAN_SEE_DETAIL = "DMGPOLIZA_CAN_SEE_DETAIL";
    public const string THIRD_LEVEL_PERMISSION_DMGPOLIZA_CAN_UNCAPITALIZE = "DMGPOLIZA_CAN_UNCAPITALIZE";
    public const string THIRD_LEVEL_PERMISSION_DMGPOLIZA_CAN_PRINT = "DMGPOLIZA_CAN_PRINT";

    // public const string THIRD_LEVEL_PERMISSION_REPORTS_CAN_SEE_HISTORICO_CUENTA = "REPORTS_CAN_SEE_HISTORICO_CUENTA";
    // public const string THIRD_LEVEL_PERMISSION_REPORTS_CAN_SEE_BALANCE_COMPROBACION = "REPORTS_CAN_SEE_BALANCE_COMPROBACION";

    // REPORTES
    public const string REPORT_TYPE_DIARIO = "diario";
    public const string REPORT_TYPE_MAYOR = "mayor";
    public const string DMGPOLIZA_PRINTED = "S";
    public const string DMGPOLIZA_NOT_PRINTED = "N";
 
    public static Margins DEFAULT_MARGINS = new Margins(10, 10, 20, 10);
    public const Size DEFAULT_REPORT_SIZE = Size.A4;
    public const Orientation DEFAULT_REPORT_ORIENTATION = Orientation.Portrait;
    public const Orientation DEFAULT_REPORT_ORIENTATION_H = Orientation.Landscape;


    public static string GetDefaultReportSwitches(string user)
    {
        return $"--footer-left \"Emitido: el {DateTime.Now:MM/dd/yyyy} a las {DateTime.Now:hh:mm tt} por {user}\" --footer-right \"[page]\" --footer-line --footer-font-size 8 --footer-spacing 13 --footer-font-name \"calibri\"";
    }

    public static string DEFAULT_REPORT_SWITCHES = "--footer-right \"[page]\" --footer-line --footer-font-size 8 --footer-spacing 13 --footer-font-name \"calibri\"";


    public const string REPORT_NAME_COMP_DIARIO = "ComprobanteDeDiarioCD";
    public const string REPORT_NAME_HISTORICO_CUENTA = "HistoricoDeCuenta";
    public const string REPORT_NAME_BALANCE_COMP = "BalanceComprobacion";

    public const string ACCOUNT_TYPE_ACTIVO = "ACTIVO";
    public const string ACCOUNT_TYPE_PASIVO = "PASIVO";
    public const string SALDO_TIPO_ACREEDOR = "A";
    public const string SALDO_TIPO_DEUDOR = "D";
}