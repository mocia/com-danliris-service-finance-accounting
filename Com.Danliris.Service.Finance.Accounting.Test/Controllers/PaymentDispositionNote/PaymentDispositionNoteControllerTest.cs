﻿using AutoMapper;
using Com.Danliris.Service.Finance.Accounting.Lib.BusinessLogic.Interfaces.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Models.PaymentDispositionNote;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.IdentityService;
using Com.Danliris.Service.Finance.Accounting.Lib.Services.ValidateService;
using Com.Danliris.Service.Finance.Accounting.Lib.Utilities;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.NewIntegrationViewModel;
using Com.Danliris.Service.Finance.Accounting.Lib.ViewModels.PaymentDispositionNoteViewModel;
using Com.Danliris.Service.Finance.Accounting.WebApi.Controllers.v1.PaymentDispositionNote;
using Com.Moonlay.NetCore.Lib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Com.Danliris.Service.Finance.Accounting.Test.Controllers.PaymentDispositionNote
{
    public class PaymentDispositionNoteControllerTest
    {
        private PaymentDispositionNoteViewModel ViewModel
        {
            get
            {
                return new PaymentDispositionNoteViewModel
                {
                    Supplier = new SupplierViewModel(),
                    AccountBank=new AccountBankViewModel(),
                    BGCheckNumber= It.IsAny<string>(),
                    BankAccountCOA= It.IsAny<string>(),
                    PaymentDispositionNo = It.IsAny<string>(),
                    PaymentDate = It.IsAny<DateTimeOffset>(),
                    Amount = It.IsAny<double>(),
                    
                    Items = new List<PaymentDispositionNoteItemViewModel>
                    {
                        new PaymentDispositionNoteItemViewModel()
                        {
                            category=new Lib.ViewModels.IntegrationViewModel.CategoryViewModel(),
                            division=new Lib.ViewModels.IntegrationViewModel.DivisionViewModel(),
                            dispositionDate= It.IsAny<DateTimeOffset>(),
                            dispositionNo=It.IsAny<string>(),
                            dispositionId=It.IsAny<string>(),
                            incomeTaxValue=It.IsAny<double>(),
                            dpp=It.IsAny<double>(),
                            vatValue=It.IsAny<double>(),
                            totalPaid=It.IsAny<double>(),
                            proformaNo=It.IsAny<string>(),
                            paymentDueDate=It.IsAny<DateTimeOffset>(),
                            payToSupplier=It.IsAny<double>(),
                            Details =new List<PaymentDispositionNoteDetailViewModel>()
                            {
                                new PaymentDispositionNoteDetailViewModel()
                                {
                                    uom=new Lib.ViewModels.IntegrationViewModel.UomViewModel(),
                                    price=It.IsAny<double>(),
                                    quantity=It.IsAny<double>(),
                                    product = new Lib.ViewModels.IntegrationViewModel.ProductViewModel(),
                                    unit = new Lib.ViewModels.IntegrationViewModel.UnitViewModel(),
                                }
                            }
                        }
                    }
                };
            }
        }
        public (Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IPaymentDispositionNoteService> Service, Mock<IMapper> Mapper) GetMocks()
        {
            return (IdentityService: new Mock<IIdentityService>(), ValidateService: new Mock<IValidateService>(), Service: new Mock<IPaymentDispositionNoteService>(), Mapper: new Mock<IMapper>());
        }

        protected PaymentDispositionNoteController GetController((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IPaymentDispositionNoteService> Service, Mock<IMapper> Mapper) mocks)
        {
            var user = new Mock<ClaimsPrincipal>();
            var claims = new Claim[]
            {
                    new Claim("username", "unittestusername")
            };
            user.Setup(u => u.Claims).Returns(claims);
            PaymentDispositionNoteController controller = new PaymentDispositionNoteController(mocks.IdentityService.Object, mocks.ValidateService.Object, mocks.Mapper.Object, mocks.Service.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user.Object
                }
            };
            controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer unittesttoken";
            controller.ControllerContext.HttpContext.Request.Path = new PathString("/v1/unit-test");
            return controller;
        }

        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        private ServiceValidationException GetServiceValidationExeption()
        {
            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.ValidationContext validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(this.ViewModel, serviceProvider.Object, null);
            return new ServiceValidationException(validationContext, validationResults);
        }

        private int GetStatusCodeGet((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IPaymentDispositionNoteService> Service, Mock<IMapper> Mapper) mocks)
        {
            PaymentDispositionNoteController controller = GetController(mocks);
            IActionResult response = controller.Get();

            return GetStatusCode(response);
        }

        [Fact]
        public void Get_WithoutException_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service
                .Setup(f => f.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ReadResponse<PaymentDispositionNoteModel>(new List<PaymentDispositionNoteModel>() {new PaymentDispositionNoteModel() }, 0, new Dictionary<string, string>(), new List<string>()));
            mocks.Mapper
                .Setup(f => f.Map<List<PaymentDispositionNoteViewModel>>(It.IsAny<List<PaymentDispositionNoteModel>>()))
                .Returns(new List<PaymentDispositionNoteViewModel>());

            var response = GetController(mocks).Get();
            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public void Get_ReadThrowException_ReturnInternalServerError()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.Read(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());

            int statusCode = GetStatusCodeGet(mocks);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        private async Task<int> GetStatusCodePost((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IPaymentDispositionNoteService> Service, Mock<IMapper> Mapper) mocks)
        {
            PaymentDispositionNoteController controller = GetController(mocks);
            IActionResult response = await controller.Post(ViewModel);

            return GetStatusCode(response);
        }

        [Fact]
        public async Task Post_WithoutException_ReturnCreated()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(s => s.Validate(It.IsAny<PaymentDispositionNoteViewModel>())).Verifiable();
            mocks.Service.Setup(s => s.CreateAsync(It.IsAny<PaymentDispositionNoteModel>())).ReturnsAsync(1);

            int statusCode = await GetStatusCodePost(mocks);
            Assert.Equal((int)HttpStatusCode.Created, statusCode);
        }

        [Fact]
        public async Task Post_ThrowException_ReturnInternalServerError()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(s => s.Validate(It.IsAny<PaymentDispositionNoteViewModel>())).Verifiable();
            mocks.Service.Setup(s => s.CreateAsync(It.IsAny<PaymentDispositionNoteModel>())).ThrowsAsync(new Exception());

            int statusCode = await GetStatusCodePost(mocks);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        //[Fact]
        //public async Task Post_BadRequest()
        //{
        //    var mocks = GetMocks();
        //    mocks.ValidateService.Setup(s => s.Validate(It.IsAny<PaymentDispositionNoteViewModel>())).Verifiable();
        //    mocks.Service.Setup(s => s.CreateAsync(It.IsAny<PaymentDispositionNoteModel>())).ReturnsAsync(1);

        //    int statusCode = await GetStatusCodePost(mocks);
        //    Assert.Equal((int)HttpStatusCode.Created, statusCode);
        //}

        [Fact]
        public void Post_Throws_Validation_Exception()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PaymentDispositionNoteViewModel>())).Throws(GetServiceValidationExeption());
            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IPaymentDispositionNoteService>();
            var mockIdentity = new Mock<IIdentityService>();
            var ViewModel = this.ViewModel;
            ViewModel.PaymentDate = DateTimeOffset.MinValue;
            var response = GetController((mockIdentity, validateMock, mockFacade, mockMapper)).Post(ViewModel).Result;
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        private async Task<int> GetStatusCodeGetById((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IPaymentDispositionNoteService> Service, Mock<IMapper> Mapper) mocks)
        {
            PaymentDispositionNoteController controller = GetController(mocks);
            IActionResult response = await controller.GetById(1);

            return GetStatusCode(response);
        }

        [Fact]
        public async Task GetById_NotNullModel_ReturnOK()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync(new PaymentDispositionNoteModel());

            int statusCode = await GetStatusCodeGetById(mocks);
            Assert.Equal((int)HttpStatusCode.OK, statusCode);
        }

        [Fact]
        public async Task GetById_NullModel_ReturnNotFound()
        {
            var mocks = GetMocks();
            mocks.Mapper.Setup(f => f.Map<PaymentDispositionNoteViewModel>(It.IsAny<PaymentDispositionNoteModel>())).Returns(ViewModel);
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ReturnsAsync((PaymentDispositionNoteModel)null);

            int statusCode = await GetStatusCodeGetById(mocks);
            Assert.Equal((int)HttpStatusCode.NotFound, statusCode);
        }

        [Fact]
        public async Task GetById_ThrowException_ReturnInternalServerError()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.ReadByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());

            int statusCode = await GetStatusCodeGetById(mocks);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCode);
        }

        [Fact]
        public void Delete_Success()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.DeleteAsync(It.IsAny<int>())).ReturnsAsync(1);

            var response = GetController(mocks).Delete(1).Result;
            Assert.Equal((int)HttpStatusCode.NoContent, GetStatusCode(response));
        }

        [Fact]
        public void Delete_Throws_Internal_Error()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.DeleteAsync(It.IsAny<int>())).Throws(new Exception());

            var response = GetController(mocks).Delete(1).Result;
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Delete_Throws_Internal_Error_Aggregate()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.DeleteAsync(It.IsAny<int>())).Throws(new AggregateException());

            var response = GetController(mocks).Delete(1).Result;
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        private async Task<int> GetStatusCodePut((Mock<IIdentityService> IdentityService, Mock<IValidateService> ValidateService, Mock<IPaymentDispositionNoteService> Service, Mock<IMapper> Mapper) mocks)
        {
            PaymentDispositionNoteController controller = GetController(mocks);
            IActionResult response = await controller.Put(ViewModel.Id,ViewModel);

            return GetStatusCode(response);
        }

        [Fact]
        public async Task Put_WithoutException_ReturnUpdated()
        {
            var mocks = GetMocks();
            mocks.ValidateService.Setup(s => s.Validate(It.IsAny<PaymentDispositionNoteViewModel>())).Verifiable();
            mocks.Service.Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<PaymentDispositionNoteModel>())).ReturnsAsync(1);

            int statusCode = await GetStatusCodePut(mocks);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCode);
        }

        [Fact]
        public void Update_Throws_Validation_Exception()
        {
            var validateMock = new Mock<IValidateService>();
            validateMock.Setup(s => s.Validate(It.IsAny<PaymentDispositionNoteViewModel>())).Throws(GetServiceValidationExeption());
            var mockMapper = new Mock<IMapper>();

            var mockFacade = new Mock<IPaymentDispositionNoteService>();
            var mockIdentity = new Mock<IIdentityService>();
            var response = GetController((mockIdentity, validateMock, mockFacade, mockMapper)).Put(It.IsAny<int>(),ViewModel).Result;
            Assert.Equal((int)HttpStatusCode.BadRequest, GetStatusCode(response));
        }

        //[Fact]
        //public void Update_Success()
        //{
        //    var mocks = GetMocks();
        //    mocks.ValidateService.Setup(s => s.Validate(It.IsAny<PaymentDispositionNoteViewModel>())).Verifiable();
        //    mocks.Service.Setup(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<PaymentDispositionNoteModel>())).ReturnsAsync(1);

        //    int statusCode = await GetStatusCodePut(mocks);
        //    Assert.Equal((int)HttpStatusCode.Created, statusCode);
        //}

        [Fact]
        public void Update_Throws_Internal_Error()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.UpdateAsync(It.IsAny<int>(), It.IsAny<PaymentDispositionNoteModel>())).Throws(new Exception());

            var response = GetController(mocks).Put(1,It.IsAny<PaymentDispositionNoteViewModel>()).Result;
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public void Update_Throws_Internal_Error_Aggregate()
        {
            var mocks = GetMocks();
            mocks.Service.Setup(f => f.UpdateAsync(It.IsAny<int>(), It.IsAny<PaymentDispositionNoteModel>())).Throws(new AggregateException());

            var response = GetController(mocks).Put(1, It.IsAny<PaymentDispositionNoteViewModel>()).Result;
            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
