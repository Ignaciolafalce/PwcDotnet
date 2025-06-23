using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PwcDotnet.Application.Interfaces;
using PwcDotnet.Domain.AggregatesModel.CustomerAggregate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PwcDotnet.Infrastructure.Services
{
    public class DurableNotificationService : INotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DurableNotificationService> _logger;
        private readonly IConfiguration _configuration;
        private readonly ICustomerRepository _customerRepository;

        public DurableNotificationService(HttpClient httpClient, ILogger<DurableNotificationService> logger, IConfiguration configuration, ICustomerRepository customerRepository)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            _customerRepository = customerRepository;
        }

        public async Task NotifyRentalCreatedAsync(int rentalId, int customerId, DateTime startDate, DateTime endDate)
        {
            try
            {

                var azureDurableFunctionsEnable = bool.Parse(_configuration["AzureDurableFunctions:Enable"] ?? "false");
                if (!azureDurableFunctionsEnable)
                {
                    _logger.LogWarning("Durable orchestration is disabled - Email orchestrator not triggered for rental {RentalId}", rentalId);
                }

                var azureDurableFunctionsUrl = _configuration["AzureDurableFunctions:Url"];

                string customerEmail = (await _customerRepository.GetByIdAsync(customerId))?.Email ?? throw new ArgumentNullException(nameof(Customer.Email));

                var body = new { rentalId, customerEmail, startDate, endDate };

                var response = await _httpClient.PostAsJsonAsync($"{azureDurableFunctionsUrl}/SendRentalEmailOrchestration_HttpStart", body);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Durable orchestration triggered for rental {RentalId}", rentalId);
            }
            catch (HttpRequestException httpRequestException) // i tried to create something to make a retry if operations fails ... :D
            {
                _logger.LogWarning("Durable orchestration triggered for rental {RentalId} fails: {message}", rentalId, httpRequestException.Message);
                Func<Task<bool>> operation = async () =>
                {
                    await this.NotifyRentalCreatedAsync(rentalId, customerId, startDate, endDate);
                    return true;
                };

                _logger.LogWarning("Retrying orchestration triggered for rental {RentalId} fails: {message}", rentalId, httpRequestException.Message);
                await ExecuteWithExponentialRetryAsync<bool>(
                    operation, 7, TimeSpan.FromHours(3), TimeSpan.FromHours(5));
            }

        }

        public static async Task<T> ExecuteWithExponentialRetryAsync<T>(
            Func<Task<T>> operation, int maxRetries = 3, TimeSpan initialDelay = default(TimeSpan), TimeSpan? maxDelay = null)
        {
            int retries = 0;
            TimeSpan delay = initialDelay == default(TimeSpan) ? TimeSpan.FromSeconds(1) : initialDelay;
            Random random = new Random();

            while (true)
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex) when (retries < maxRetries)
                {
                    retries++;
                    await Task.Delay(delay);
                    delay = TimeSpan.FromSeconds(Math.Pow(2, retries)) + TimeSpan.FromMilliseconds(random.Next(0, 100));

                    if (maxDelay != null && delay > maxDelay.Value)
                    {
                        delay = maxDelay.Value;
                    }
                    else if (retries == maxRetries)
                    {
                        throw new Exception("Failed to execute operation after " + retries + " retries.", ex);
                    }
                }
            }
        }


    }

}
