//using Moq;
//using CoreFinance.Application.DTOs;
//using CoreFinance.Application.Services;
//using CoreFinance.Domain;

//namespace CoreFinance.Application.Tests
//{
//    public class AccountServiceTests
//    {
//        private readonly Mock<IAccountRepository> _repoMock;
//        private readonly AccountService _service;

//        public AccountServiceTests()
//        {
//            _repoMock = new Mock<IAccountRepository>();
//            _service = new AccountService(_repoMock.Object);
//        }

//        [Fact]
//        public async Task CreateAccountAsync_ShouldReturnAccountDto()
//        {
//            // Arrange
//            var request = new CreateAccountRequest
//            {
//                UserId = Guid.NewGuid(),
//                AccountName = "Test Account",
//                AccountType = "Bank",
//                Currency = "VND",
//                InitialBalance = 1000
//            };

//            // Act
//            var result = await _service.CreateAccountAsync(request);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(request.AccountName, result.AccountName);
//            Assert.Equal(request.Currency, result.Currency);
//            Assert.Equal(request.InitialBalance, result.InitialBalance);
//        }

//        [Fact]
//        public async Task GetAccountsAsync_ShouldReturnList()
//        {
//            // Arrange
//            var userId = Guid.NewGuid();
//            var accounts = new List<Account>
//            {
//                new() { AccountId = Guid.NewGuid(), UserId = userId, AccountName = "A", AccountType = AccountType.Bank, Currency = "VND", InitialBalance = 100, CurrentBalance = 100, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IsActive = true },
//                new() { AccountId = Guid.NewGuid(), UserId = userId, AccountName = "B", AccountType = AccountType.Cash, Currency = "USD", InitialBalance = 200, CurrentBalance = 200, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IsActive = true }
//            };
//            _repoMock.Setup(r => r.GetAccountsByUserIdAsync(userId)).ReturnsAsync(accounts);

//            // Act
//            var result = await _service.GetAccountsAsync(userId);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Collection(result,
//                a => Assert.Equal("A", a.AccountName),
//                a => Assert.Equal("B", a.AccountName));
//        }
//    }
//} 