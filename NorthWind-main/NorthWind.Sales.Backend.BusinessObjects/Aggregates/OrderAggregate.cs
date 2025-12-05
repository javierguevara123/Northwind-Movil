//using NorthWind.Sales.Backend.BusinessObjects.Enums;
//using NorthWind.Sales.Backend.BusinessObjects.POCOEntities;
//using NorthWind.Sales.Backend.BusinessObjects.ValueObjects;
//using NorthWind.Sales.Entities.Dtos.CreateOrder;

using NorthWind.Sales.Entities.Dtos.Orders.CreateOrder;

namespace NorthWind.Sales.Backend.BusinessObjects.Aggregates;

// ************************************
//  * Aggregate                       *
// ************************************
//  Función: Es un patron de diseño que permite mantener una lista de objetos (Ejemplo: un
//  maestro/detalle.
//  El "Agregado" debe tener un objeto principal llamado "Root".
//  Para definir en el agregado el objeto "Root" se puede:
//  a) Utilizar herencia como en este ejemplo.
//  b) Utilizar una propiedad que sea de tipo: Order
//  c) Crear una interfaz llamada por ejemplo: IAggregate y en la clase actual implementar
//  esta interfaz.
public class OrderAggregate : Order
{
  //  Campo privado y de solo lectura para que no se pueda agregar datos desde el exterior.
  //  En el ejemplo Se utiliza inicializadores de colección.
  //  NOTA: OrderDetailsField es el otro objeto que integra el aggregate y representa los
  //  detalles de la orden (OrderDetail).
  private readonly List<OrderDetail> OrderDetailsField = [];

  //  Acceso solo de lectura a los datos. Esta propiedad permite devolver el contenido
  //  del campo: "OrderDetailsField" para si se requiere realizar consultas de los detalles
  //  de la orden.
  public IReadOnlyCollection<OrderDetail> OrderDetails => OrderDetailsField;

  // Código anterior equivalente
  //private readonly List<OrderDetail> OrderDetailsField = new List<OrderDetail>();

  //public IEnumerable<OrderDetail> OrderDetails
  //{
  //  get { return OrderDetailsField.AsReadOnly();}
  //}

  //  Este es el método que permite agregar un detalle de la orden.
  //  Se va agregando cada detalle de la orden y se valida que se cumpla con la regla de negocio.
  //  NOTA: El método "AddDetail" es la única forma de agregar un detalle a una orden.
  public void AddDetail(int productId, decimal unitPrice, short quantity)
  {
    //  Verificar antes de agregar el producto al detalle si el producto ya esta en la lista
    //  si es asi sumamos la cantidad al producto existente.
    var ExistingOrderDetail = OrderDetailsField.FirstOrDefault(o => o.ProductId == productId);

    if (ExistingOrderDetail != default)
    {
      //  Obtener la nueva cantidad.
      quantity += ExistingOrderDetail.Quantity;
      //  Eliminar el detalle anterior.
      OrderDetailsField.Remove(ExistingOrderDetail);
    }

    //  Agregar el nuevo detalle a la orden.
    OrderDetailsField.Add(new OrderDetail(productId, unitPrice, quantity));
  }

    //  Del exterior (UI) se recibe un "Dto" con los datos de la orden.
    //  Este método permite "CONVERTIR" un "Dto" en un aggregate. 
    //  Esto se puede hacer con un framework como "Automapper".
    //  Se regresa un "OrderAggregate" a partir de un "CreateOrderDto".
    public static OrderAggregate From(CreateOrderDto orderDto, string userId)
    {
        OrderAggregate OrderAggregate = new OrderAggregate
        {
            // Asignamos el ID del usuario autenticado
            UserId = userId,

            ShipAddress = orderDto.ShipAddress,
            ShipCity = orderDto.ShipCity,
            ShipCountry = orderDto.ShipCountry,
            ShipPostalCode = orderDto.ShipPostalCode
        };

        foreach (var Item in orderDto.OrderDetails)
        {
            OrderAggregate.AddDetail(Item.ProductId, Item.UnitPrice, Item.Quantity);
        }

        return OrderAggregate;
    }
}
