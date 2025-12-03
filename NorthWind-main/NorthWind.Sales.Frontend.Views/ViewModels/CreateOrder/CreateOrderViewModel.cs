using NorthWind.RazorComponents.Validators;
using NorthWind.Sales.Entities.Dtos.Orders.CreateOrder;
using NorthWind.Sales.Frontend.BusinessObjects.Interfaces;
using NorthWind.Sales.Frontend.Views.Resources;
using NorthWind.Validation.Entities.Interfaces;
using NorthWind.Validation.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthWind.Sales.Frontend.Views.ViewModels.CreateOrder
{
    public class CreateOrderViewModel(ICreateOrderGateway gateway,
 IModelValidatorHub<CreateOrderViewModel> validator)
    {
        public IModelValidatorHub<CreateOrderViewModel> Validator => validator;
        #region Propiedades relacionadas a CreateOrderDto
        public string CustomerId { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipCountry { get; set; }
        public string ShipPostalCode { get; set; }
        public List<CreateOrderDetailViewModel> OrderDetails { get; set; } = [];
        #endregion
        public string InformationMessage { get; private set; }

        public ModelValidator<CreateOrderViewModel>
 ModelValidatorComponentReference { get; set; }
        public void AddNewOrderDetailItem()
        {
            OrderDetails.Add(
            new CreateOrderDetailViewModel());
        }
        public async Task Send()
        {
            InformationMessage = "";
            try
            {
                var OrderId = await gateway.CreateOrderAsync(
                (CreateOrderDto)this);
                InformationMessage = string.Format(
                CreateOrderMessages.CreatedOrderTemplate, OrderId);
            }
            catch (HttpRequestException ex)
            {
                if (ex.Data.Contains("Errors"))
                {
                    IEnumerable<ValidationError> Errors =
                    ex.Data["Errors"] as IEnumerable<ValidationError>;
                    ModelValidatorComponentReference.AddErrors(Errors);
                }
                else
                {
                    throw;
                }
            }
        }

        public static explicit operator CreateOrderDto(
       CreateOrderViewModel model) =>
       new CreateOrderDto(
       model.CustomerId, model.ShipAddress, model.ShipCity,
       model.ShipCountry, model.ShipPostalCode,
       model.OrderDetails.Select(d => new CreateOrderDetailDto(
       d.ProductId, d.UnitPrice, d.Quantity)
       ));
    }
}
