using CurrencyConversion.DataAccessLayer.Data;
using CurrencyConversion.Service.Rates;
using CurrencyConversion.Service.Repository.IRepository;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CurrencyConversion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly log4net.ILog _log;

        public RatesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _log = LogManager.GetLogger(typeof(RatesController));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var rates = await _unitOfWork.ExchangeRates.GetAll();
                return Ok(rates);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return BadRequest("Something went wrong!");
            }
        }

        [Authorize]
        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code)) return BadRequest();
                var rate = await _unitOfWork.ExchangeRates.Get(r => r.CurrencyCode == code.ToUpperInvariant());
                if (rate == null) return NotFound($"Invalied code : {code}");
                return Ok(new { rate.CurrencyCode, rate.DkkPerUnit, rate.AsOfDate });
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return BadRequest("Something went wrong!");
            }
        }
    }
}
