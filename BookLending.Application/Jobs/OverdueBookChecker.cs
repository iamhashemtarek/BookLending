using BookLending.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Application.Jobs
{
    public class OverdueBookChecker
    {
        private readonly IBorrowService _borrowService;
        private readonly ILogger<OverdueBookChecker> _logger;

        public OverdueBookChecker(IBorrowService borrowService, ILogger<OverdueBookChecker> logger)
        {
            _borrowService = borrowService;
            _logger = logger;
        }

        public async Task CheckOverdueBooksAsync()
        {
            _logger.LogInformation("Starting overdue book check job");
            try
            {
                await _borrowService.CheckOverdueBorrowsAsync();
                _logger.LogInformation("Overdue book check completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for overdue books.");
            }
        }
    }
}
