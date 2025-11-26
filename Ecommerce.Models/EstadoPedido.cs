using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Models
{
    public enum EstadoPedido
    {
        Pendiente,    // El cliente acaba de crearlo
        Procesando,   // Pago recibido, en almacén
        Enviado,      // En manos del transportista
        Completado,   // El cliente lo recibió
        Cancelado     // Pedido cancelado
    }
}
