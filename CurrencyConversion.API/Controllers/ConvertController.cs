using CurrencyConversion.API.Models;
using CurrencyConversion.Domain.Entities;
using CurrencyConversion.DataAccessLayer.Data;
using CurrencyConversion.Service.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CurrencyConversion.Service.Rates;
using log4net;

namespace CurrencyConversion.API.Controllers
{
    [Route("api/[controller]")]

    [ApiController]
    public class ConvertController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly log4net.ILog _log;

        public ConvertController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _log = LogManager.GetLogger(typeof(ConvertController));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Convert([FromBody] ConversionRequest req)
        {
            try
            {
                if (req is null || string.IsNullOrWhiteSpace(req.Currency) || req.Amount <= 0)
                    return BadRequest("Provide a currency code and a positive amount.");

                var code = req.Currency.ToUpperInvariant();
                var rate = await _unitOfWork.ExchangeRates.Get(r => r.CurrencyCode == code);
                if (rate == null) return NotFound($"No rate for {code}.");

                var dkk = decimal.Round(req.Amount * rate.DkkPerUnit, 2);

                var record = new ConversionRecord
                {
                    FromCurrency = code,
                    InputAmount = req.Amount,
                    OutputDkk = dkk,
                    RateUsed = rate.DkkPerUnit,
                    AsOfDate = rate.AsOfDate,
                    CreatedUtc = DateTime.UtcNow
                };

                _unitOfWork.Conversions.Add(record);
                _unitOfWork.Save();

                return Ok(record);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return BadRequest("Something went wrong!");
            }
        }

        // GET /api/convert/history?currency=USD&from=2025-09-01&to=2025-09-08
        [Authorize(Roles = "Admin")]
        [HttpGet("history")]
        public async Task<IActionResult> History(HistoryRequest historyRequest)
        {
            try
            {
                var q = await _unitOfWork.Conversions.GetAll();
                if (!string.IsNullOrWhiteSpace(historyRequest.Currency)) q = [.. q.Where(x => x.FromCurrency == historyRequest.Currency.ToUpperInvariant())];
                if (historyRequest.FromDate.ToString() != null && historyRequest.ToDate.ToString() != null)
                {
                    if (historyRequest.FromDate >= historyRequest.ToDate) return BadRequest("fromDate should be less than ToDate");
                }
                if (string.IsNullOrEmpty(historyRequest.FromDate.ToString())) q = [.. q.Where(x => x.CreatedUtc >= historyRequest.FromDate)];
                if (string.IsNullOrEmpty(historyRequest.ToDate.ToString())) q = [.. q.Where(x => x.CreatedUtc <= historyRequest.ToDate)];

                var list = q.OrderByDescending(x => x.CreatedUtc);
                return Ok(list);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return BadRequest("Something went wrong!");
            }
        }

    }
}
