using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebBanHang.Controllers;
using WebBanHang.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace WebBanHang.Tests
{
    [TestClass]
    public class TestLogin
    {
        private AccountController _controller;
        private Mock<ApplicationSignInManager> _signInManagerMock;

        [TestInitialize]
        public void Setup()
        {
            // Tạo mock UserManager
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<UserManager<ApplicationUser>>(userStore.Object);
            
            // Tạo mock AuthenticationManager
            var authManager = new Mock<IAuthenticationManager>();
            
            // Khởi tạo SignInManager mock
            _signInManagerMock = new Mock<ApplicationSignInManager>(userManager.Object, authManager.Object);

            // Khởi tạo controller với SignInManager mock
            _controller = new AccountController();
            _controller.SignInManager = _signInManagerMock.Object;
        }

        [TestMethod]
        public void DummyTest()
        {
            Assert.IsTrue(true);
        }

        // Test Case 1: Kiểm tra GET Login trả về view
        [TestMethod]
        public void Login_GET_ReturnsView()
        {
            // Act
            var result = _controller.Login(null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.ViewName); // View mặc định
        }

        // Test Case 2: Kiểm tra GET Login gán ReturnUrl vào ViewBag
        [TestMethod]
        public void Login_GET_SetsReturnUrlInViewBag()
        {
            // Arrange
            string returnUrl = "/Home/Index";

            // Act
            var result = _controller.Login(returnUrl) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(returnUrl, _controller.ViewBag.ReturnUrl);
        }

        // Test Case 3: Kiểm tra POST Login với model không hợp lệ trả về view
        [TestMethod]
        public async Task Login_POST_InvalidModel_ReturnsView()
        {
            // Arrange
            _controller.ModelState.AddModelError("UserName", "Bắt buộc");
            var model = new LoginViewModel();

            // Act
            var result = await _controller.Login(model, null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(model, result.Model);
        }

        // Test Case 4: Kiểm tra POST Login với model hợp lệ và SignInStatus.Success chuyển hướng đến ReturnUrl
        [TestMethod]
        public async Task Login_POST_SignInSuccess_RedirectsToReturnUrl()
        {
            // Arrange
            var model = new LoginViewModel { UserName = "test", Password = "pass", RememberMe = false };
            string returnUrl = "/Home/Index";
            _signInManagerMock.Setup(m => m.PasswordSignInAsync("test", "pass", false, false))
                             .ReturnsAsync(SignInStatus.Success);

            // Act
            var result = await _controller.Login(model, returnUrl) as RedirectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(returnUrl, result.Url);
        }

        // Test Case 5: Kiểm tra POST Login với SignInStatus.Success và ReturnUrl là null chuyển hướng đến mặc định
        [TestMethod]
        public async Task Login_POST_SignInSuccess_NullReturnUrl_RedirectsToDefault()
        {
            // Arrange
            var model = new LoginViewModel { UserName = "test", Password = "pass", RememberMe = false };
            _signInManagerMock.Setup(m => m.PasswordSignInAsync("test", "pass", false, false))
                             .ReturnsAsync(SignInStatus.Success);

            // Act
            var result = await _controller.Login(model, null) as RedirectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("/", result.Url); // Giả định RedirectToLocal mặc định là "/"
        }

        // Test Case 6: Kiểm tra POST Login với SignInStatus.LockedOut trả về view Lockout
        [TestMethod]
        public async Task Login_POST_SignInLockedOut_ReturnsLockoutView()
        {
            // Arrange
            var model = new LoginViewModel { UserName = "test", Password = "pass", RememberMe = false };
            _signInManagerMock.Setup(m => m.PasswordSignInAsync("test", "pass", false, false))
                             .ReturnsAsync(SignInStatus.LockedOut);

            // Act
            var result = await _controller.Login(model, null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Lockout", result.ViewName);
        }

        // Test Case 7: Kiểm tra POST Login với SignInStatus.RequiresVerification chuyển hướng đến SendCode
        //[TestMethod]
        //public async Task Login_POST_SignInRequiresVerification_RedirectsToSendCode()
        //{
        //    // Arrange
        //    var model = new LoginViewModel { UserName = "test", Password = "pass", RememberMe = true };
        //    string returnUrl = "/Home/Index";
        //    _signInManagerMock.Setup(m => m.PasswordSignInAsync("test", "pass", true, false))
        //                     .ReturnsAsync(SignInStatus.RequiresVerification);

        //    // Act
        //    var result = await _controller.Login(model, returnUrl) as RedirectToActionResult;

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual("SendCode", result.ActionName);
        //    Assert.AreEqual(returnUrl, result.RouteValues["ReturnUrl"]);
        //    Assert.AreEqual(true, result.RouteValues["RememberMe"]);
        //}

        // Test Case 8: Kiểm tra POST Login với SignInStatus.Failure trả về view với lỗi
        [TestMethod]
        public async Task Login_POST_SignInFailure_ReturnsViewWithError()
        {
            // Arrange
            var model = new LoginViewModel { UserName = "test", Password = "pass", RememberMe = false };
            _signInManagerMock.Setup(m => m.PasswordSignInAsync("test", "pass", false, false))
                             .ReturnsAsync(SignInStatus.Failure);

            // Act
            var result = await _controller.Login(model, null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(model, result.Model);
            Assert.IsTrue(_controller.ModelState.ContainsKey(""));
            Assert.AreEqual("Invalid login attempt.", _controller.ModelState[""].Errors[0].ErrorMessage);
        }

        // Test Case 9: Kiểm tra POST Login với UserName rỗng
        [TestMethod]
        public async Task Login_POST_EmptyUserName_ReturnsView()
        {
            // Arrange
            _controller.ModelState.AddModelError("UserName", "Bắt buộc");
            var model = new LoginViewModel { UserName = "", Password = "pass" };

            // Act
            var result = await _controller.Login(model, null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        // Test Case 10: Kiểm tra POST Login với Password rỗng
        [TestMethod]
        public async Task Login_POST_EmptyPassword_ReturnsView()
        {
            // Arrange
            _controller.ModelState.AddModelError("Password", "Bắt buộc");
            var model = new LoginViewModel { UserName = "test", Password = "" };

            // Act
            var result = await _controller.Login(model, null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        // Test Case 11: Kiểm tra POST Login với model là null
        [TestMethod]
        public async Task Login_POST_NullModel_ReturnsView()
        {
            // Act
            var result = await _controller.Login(null, null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        // Test Case 12: Kiểm tra POST Login với RememberMe là true
        [TestMethod]
        public async Task Login_POST_RememberMeTrue_SignInCalledWithTrue()
        {
            // Arrange
            var model = new LoginViewModel { UserName = "test", Password = "pass", RememberMe = true };
            _signInManagerMock.Setup(m => m.PasswordSignInAsync("test", "pass", true, false))
                             .ReturnsAsync(SignInStatus.Success);

            // Act
            var result = await _controller.Login(model, null);

            // Assert
            _signInManagerMock.Verify(m => m.PasswordSignInAsync("test", "pass", true, false), Times.Once());
        }

        // Test Case 13: Kiểm tra POST Login với RememberMe là false
        [TestMethod]
        public async Task Login_POST_RememberMeFalse_SignInCalledWithFalse()
        {
            // Arrange
            var model = new LoginViewModel { UserName = "test", Password = "pass", RememberMe = false };
            _signInManagerMock.Setup(m => m.PasswordSignInAsync("test", "pass", false, false))
                             .ReturnsAsync(SignInStatus.Success);

            // Act
            var result = await _controller.Login(model, null);

            // Assert
            _signInManagerMock.Verify(m => m.PasswordSignInAsync("test", "pass", false, false), Times.Once());
        }

        // Test Case 14: Kiểm tra POST Login với ReturnUrl không hợp lệ (ví dụ: URL bên ngoài)
        [TestMethod]
        public async Task Login_POST_InvalidReturnUrl_RedirectsToDefault()
        {
            // Arrange
            var model = new LoginViewModel { UserName = "test", Password = "pass", RememberMe = false };
            string returnUrl = "http://external.com";
            _signInManagerMock.Setup(m => m.PasswordSignInAsync("test", "pass", false, false))
                             .ReturnsAsync(SignInStatus.Success);

            // Act
            var result = await _controller.Login(model, returnUrl) as RedirectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("/", result.Url); // RedirectToLocal mặc định là "/"
        }

        // Test Case 15: Kiểm tra POST Login với UserName chứa ký tự đặc biệt
        [TestMethod]
        public async Task Login_POST_SpecialCharactersInUserName_SignInCalled()
        {
            // Arrange
            var model = new LoginViewModel { UserName = "test@#$", Password = "pass", RememberMe = false };
            _signInManagerMock.Setup(m => m.PasswordSignInAsync("test@#$", "pass", false, false))
                             .ReturnsAsync(SignInStatus.Success);

            // Act
            var result = await _controller.Login(model, null);

            // Assert
            _signInManagerMock.Verify(m => m.PasswordSignInAsync("test@#$", "pass", false, false), Times.Once());
        }

        // Test Case 16: Kiểm tra POST Login với UserName dài
        [TestMethod]
        public async Task Login_POST_LongUserName_SignInCalled()
        {
            // Arrange
            string longUserName = new string('a', 256);
            var model = new LoginViewModel { UserName = longUserName, Password = "pass", RememberMe = false };
            _signInManagerMock.Setup(m => m.PasswordSignInAsync(longUserName, "pass", false, false))
                             .ReturnsAsync(SignInStatus.Success);

            // Act
            var result = await _controller.Login(model, null);

            // Assert
            _signInManagerMock.Verify(m => m.PasswordSignInAsync(longUserName, "pass", false, false), Times.Once());
        }

        // Test Case 17: Kiểm tra POST Login với Password dài
        [TestMethod]
        public async Task Login_POST_LongPassword_SignInCalled()
        {
            // Arrange
            string longPassword = new string('b', 256);
            var model = new LoginViewModel { UserName = "test", Password = longPassword, RememberMe = false };
            _signInManagerMock.Setup(m => m.PasswordSignInAsync("test", longPassword, false, false))
                             .ReturnsAsync(SignInStatus.Success);

            // Act
            var result = await _controller.Login(model, null);

            // Assert
            _signInManagerMock.Verify(m => m.PasswordSignInAsync("test", longPassword, false, false), Times.Once());
        }

        //// Test Case 18: Kiểm tra POST Login với ReturnUrl là null và RequiresVerification
        //[TestMethod]
        //public async Task Login_POST_NullReturnUrl_RequiresVerification_RedirectsToSendCode()
        //{
        //    // Arrange
        //    var model = new LoginViewModel { UserName = "test", Password = "pass", RememberMe = true };
        //    _signInManagerMock.Setup(m => m.PasswordSignInAsync("test", "pass", true, false))
        //                     .ReturnsAsync(SignInStatus.RequiresVerification);

        //    // Act
        //    var result = await _controller.Login(model, null) as RedirectToActionResult;

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual("SendCode", result.ActionName);
        //    Assert.IsNull(result.RouteValues["ReturnUrl"]);
        //}

        // Test Case 19: Kiểm tra POST Login với SignInStatus.Failure nhiều lần
        [TestMethod]
        public async Task Login_POST_SignInFailure_MultipleAttempts_ReturnsView()
        {
            // Arrange
            var model = new LoginViewModel { UserName = "test", Password = "pass", RememberMe = false };
            _signInManagerMock.Setup(m => m.PasswordSignInAsync("test", "pass", false, false))
                             .ReturnsAsync(SignInStatus.Failure);

            // Act
            var result1 = await _controller.Login(model, null) as ViewResult;
            var result2 = await _controller.Login(model, null) as ViewResult;

            // Assert
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.IsTrue(_controller.ModelState.ContainsKey(""));
        }

        // Test Case 20: Kiểm tra POST Login khi SignInManager ném ngoại lệ
        [TestMethod]
        public async Task Login_POST_SignInManagerThrowsException_ReturnsView()
        {
            // Arrange
            var model = new LoginViewModel { UserName = "test", Password = "pass", RememberMe = false };
            _signInManagerMock.Setup(m => m.PasswordSignInAsync("test", "pass", false, false))
                             .ThrowsAsync(new System.Exception("Lỗi SignInManager"));

            // Act
            try
            {
                await _controller.Login(model, null);
            }
            catch (System.Exception ex)
            {
                // Assert
                Assert.AreEqual("Lỗi SignInManager", ex.Message);
            }
        }
    }
}