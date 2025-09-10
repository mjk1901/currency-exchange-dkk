using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq.Protected;
using CurrencyConversion.Service.Rates;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CurrencyConversion.Domain.Entities;
using CurrencyConversion.API.Controllers;
using CurrencyConversion.API.Models;
using Microsoft.AspNetCore.Mvc;
using CurrencyConversion.DataAccessLayer.Data;
using CurrencyConversion.Service.Repository;

namespace CurrencyConversion.Tests
{
    public class CurrencyConversionTests
    {
        [Fact]
        public async Task GetLatestAsync_ShouldReturnRatesDictionary()
        {
            // Arrange: mock HTTP response
            var xml = @"<dailyrates id='2025-09-08'>
                        <currency code='USD' rate='750'/>
                    </dailyrates>";
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(xml)
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var cfg = new ConfigurationBuilder()
                        .AddInMemoryCollection(new Dictionary<string, string>{
                        {"Nationalbanken:RatesXmlUrl","https://dummy"}
                        }).Build();
            
            var client = new NationalbankenClient(httpClient, cfg);

            // Act
            var (date, rates) = await client.GetLatestAsync(CancellationToken.None);

            // Assert
            rates.Should().ContainKey("USD");
            rates["USD"].Should().Be(7.5m); // 750 / 100
            rates["DKK"].Should().Be(1.0m);
        }

        [Fact]
        public async Task Convert_ShouldReturnConversionRecord()
        {
            var options = new DbContextOptionsBuilder<CurrencyDbContext>()
                .UseInMemoryDatabase("CurrencyDb")
                .Options;
            var db = new CurrencyDbContext(options);

            db.ExchangeRates.Add(new ExchangeRate { CurrencyCode = "USD", DkkPerUnit = 7.5m, AsOfDate = DateTime.UtcNow, UpdatedUtc = DateTime.UtcNow });
            await db.SaveChangesAsync();

            var uow = new UnitOfWork(db);
            var controller = new ConvertController(uow);

            var req = new ConversionRequest {Currency = "USD", Amount = 100 };
            var result = await controller.Convert(req) as OkObjectResult;

            result.Should().NotBeNull();
            var response = result!.Value as ConversionRecord;

            response.Should().NotBeNull();
            response.OutputDkk.Should().Be(750m);
            response.RateUsed.Should().Be(7.5m);
            response.FromCurrency.Should().Be("USD");

            // Validate response stored in DB
            var storedresponse = await db.Conversions.FirstOrDefaultAsync();
            storedresponse.Should().NotBeNull();
            storedresponse.OutputDkk.Should().Be(750m);
        }

        [Fact]
        public async Task Convert_ShouldReturnNotFound_ForUnknownCurrency()
        {
            var options = new DbContextOptionsBuilder<CurrencyDbContext>()
                .UseInMemoryDatabase("CurrencyDb")
                .Options;
            var db = new CurrencyDbContext(options);

            var uow = new UnitOfWork(db);
            var controller = new ConvertController(uow);

            var req = new ConversionRequest { Currency = "XYZ", Amount = 100 };
            var result = await controller.Convert(req);

            result.Should().BeOfType<NotFoundObjectResult>();
            var notFound = result as NotFoundObjectResult;
            notFound!.Value.Should().Be("No rate for XYZ.");
        }
    }
}