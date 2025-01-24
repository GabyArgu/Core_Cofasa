using CoreContable.Models;
using CoreContable.Models.Dto;
using CoreContable.Models.ResultSet;
using CoreContable.Services;
using CoreContable.Utils;
using CoreContable.Utils.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CoreContable.Controllers {
    public class CiasController(
        ICiasRepository ciasRepository,
        ISecurityRepository securityRepository,
        ICurrencyRepository currencyRepository,
        IDmgCuentasRepository dmgCuentasRepository,
        ILogger<CiasController> logger
    ) : Controller {
        [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_CIAS)]
        public IActionResult Index() {
            return View();
        }

        [IsAuthorized(alias: CC.SECOND_LEVEL_PERMISSION_ADMIN_CIAS)]
        [HttpGet]
        public async Task<JsonResult> GetCias() {
            List<CiaResultSet>? data;

            try {
                data = await ciasRepository.CallGetCias("1");
            }
            catch (Exception e) {
                data = null;
                logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                    nameof(CiasController), nameof(GetCias));
            }

            return Json(new {
                success = true,
                message = "Access data",
                data
            });
        }

        [IsAuthorized(alias: CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_ADD)]
        [HttpPost]
        public async Task<JsonResult> SaveCia([FromForm] CiaDto data) {
            bool result;

            try {
                if (!data.COD_CIA.IsNullOrEmpty()) {
                    result = await UpdateCia(data);
                }
                else {
                    result = await DoSaveCia(data);
                }
            }
            catch (Exception e) {
                result = false;
                logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                    nameof(CiasController), nameof(SaveCia));
            }

            return Json(new {
                success = result,
                message = result ? "Compañía guardada" : "Ocurrió un error al guardar el registro",
            });
        }

        private async Task<bool> DoSaveCia(CiaDto data) {
            bool result;

            try {
                var codCia = await ciasRepository.CallGenerateCiaCod();

                if (!codCia.IsNullOrEmpty()) {
                    data.COD_CIA = codCia;
                    result = await ciasRepository.CallSaveCia(cia: data);
                }
                else {
                    result = false;
                }
            }
            catch (Exception e) {
                result = false;
                logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                    nameof(CiasController), nameof(DoSaveCia));
            }

            return result;
        }

        [IsAuthorized(alias: CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_COPY)]
        [HttpPost]
        public async Task<JsonResult> CopyCia([FromForm] CiaDto data) {
            bool result;

            try {
                if (data.COD_CIA != null) {
                    data.COD_CIA = null;
                }

                result = await DoSaveCia(data);
            }
            catch (Exception e) {
                result = false;
                logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                    nameof(CiasController), nameof(CopyCia));
            }

            return Json(new {
                success = result,
                message = result ? "Compañía copiada correctamente" : "Ocurrió un error al copiar el registro",
            });
        }

        [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_UPDATE}," +
                             $"{CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_COPY}")]
        private async Task<bool> UpdateCia(CiaDto data) {
            bool result;

            try {
                result = await ciasRepository.CallUpdateCia(data);
            }
            catch (Exception e) {
                result = false;
                logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                    nameof(CiasController), nameof(UpdateCia));
            }

            return result;
        }

        [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_UPDATE}," +
                             $"{CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_COPY}")]
        [HttpGet]
        public async Task<JsonResult> GetCiaById([FromQuery] string ciaCod) {
            bool result;
            CiaResultSet? cia = null;

            try {
                cia = await ciasRepository.GetOneCia(ciaCod);
                result = true;

                if (cia is { CodMoneda: not null }) {
                    var acMonMoneda = await currencyRepository.GetCurrencyById(cia.CodMoneda);
                    if (acMonMoneda != null) {
                        cia.MonedaSelect2 = new Select2ResultSet {
                            id = acMonMoneda.MonCodigo,
                            text = $"({acMonMoneda.MonSiglas}/{acMonMoneda.MonSimbolo}) - {acMonMoneda.MonNombre}"
                        };
                    }
                }
            }
            catch (Exception e) {
                result = false;
                logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                    nameof(CiasController), nameof(GetCiaById));
            }

            return Json(new {
                success = result,
                message = result ? "Access data" : "Ocurrió un error al obetener la empresa",
                data = cia
            });
        }

        [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_ADD}," +
                                $"{CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_UPDATE}," +
                                $"{CC.THIRD_LEVEL_PERMISSION_DMGCUENTAS_CAN_ADD}," +
                                $"{CC.THIRD_LEVEL_PERMISSION_DMGCUENTAS_CAN_UPDATE}," +
                                $"{CC.THIRD_LEVEL_PERMISSION_CIAS_CAN_COPY}")] 

        [HttpPost]
        public async Task<JsonResult> GetAccountName([FromForm] GetAccountNameDto data) {
            bool result;
            var accountName = "";

            try {
                var actualCiaCod = securityRepository.GetSessionCiaCode();
                data.CodCia = actualCiaCod;
                accountName = await dmgCuentasRepository.GetName(data);
                result = !accountName.IsNullOrEmpty();
            }
            catch (Exception e) {
                result = false;
                logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                    nameof(CiasController), nameof(GetAccountName));
            }

            return Json(new {
                success = result,
                message = result ? "Access data" : "Ocurrió un error al obtener el registro",
                data = accountName
            });
        }

        [IsAuthorized(alias: $"{CC.THIRD_LEVEL_PERMISSION_USERS_CAN_ADD}," +
                             $"{CC.THIRD_LEVEL_PERMISSION_USERS_CAN_UPDATE}" +
                             $"{CC.THIRD_LEVEL_PERMISSION_USERS_CAN_COMPANY}")]
        [HttpGet]
        public async Task<JsonResult> GetToSelect2([FromQuery] string q) {
            var cias = new List<Select2ResultSet>();

            try {
                cias = await ciasRepository.CallGetCiasForSelect2(q);
            }
            catch (Exception e) {
                logger.LogError(e, "Ocurrió un error en {Class}.{Method}",
                    nameof(CiasController), nameof(GetToSelect2));
            }

            return Json(new {
                results = cias
            });
        }
    }
}