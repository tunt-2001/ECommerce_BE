using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
    {
        private readonly IApplicationDbContext _context;
        public CreateOrderCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order
            {
                UserId = request.UserId,
                ShippingAddress = request.ShippingAddress,
                OrderDate = DateTime.UtcNow,
                Status = "Pending"
            };

            decimal totalAmount = 0;

            foreach (var item in request.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || product.Stock < item.Quantity)
                {
                    throw new Exception($"Product with Id {item.ProductId} is not available in the requested quantity.");
                }

                var orderDetail = new OrderDetail
                {
                    Order = order,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price
                };
                order.OrderDetails.Add(orderDetail);

                product.Stock -= item.Quantity; // Giảm tồn kho
                totalAmount += product.Price * item.Quantity;
            }

            order.TotalAmount = totalAmount;

            await _context.Orders.AddAsync(order, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return order.Id;
        }
    }
}
