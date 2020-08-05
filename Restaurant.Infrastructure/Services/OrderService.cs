using Org.BouncyCastle.Asn1.Cms;
using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.RabbitMQ;
using Restaurant.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly ICartMealRepository _cartMealRepository;
        private readonly IPublisher _publisher;
        private readonly IUserRepository _userRepository;

        public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository, ICartItemRepository cartItemRepository, ICartMealRepository cartMealRepository, IPublisher publisher, IUserRepository userRepository)
        {
            _orderRepository = orderRepository ?? throw new NotImplementedException("Order repository");
            _cartRepository = cartRepository ?? throw new NotImplementedException("Cart repository");
            _cartItemRepository = cartItemRepository ?? throw new NotImplementedException("Cart item repository");
            _cartMealRepository = cartMealRepository ?? throw new NotImplementedException("Cart meal repository");
            _publisher = publisher ?? throw new NotImplementedException("Publisher");
            _userRepository = userRepository;
        }

        public async Task<Order> Process(ulong userId, int paymentMethod, List<Item> items, List<Meal> meals, decimal price)
        {
            User user = await _userRepository.SingleAsync(user => user.UserId == userId);
            if (user == null)
                return new Order();

            // Create cart
            var cart = new Cart()
            {
                UserId = user.UserId
            };
            await _cartRepository.Add(cart);

            // Fetch newly created id of cart
            ulong cartId = (await _cartRepository.SingleAsync(cart => cart.UserId == user.UserId)).CartId;

            // Create cart-item junctions
            if (items.Count > 0)
                foreach (var item in items)
                {
                    var cartItem = item.CartItem.ElementAt(0);
                    cartItem.CartId = cartId;
                    await _cartItemRepository.Add(cartItem);
                }

            // Create cart-meal junctions
            if (meals.Count > 0)
                foreach (var meal in meals)
                {
                    var cartMeal = meal.CartMeal.ElementAt(0);
                    cartMeal.CartId = cartId;
                    await _cartMealRepository.Add(cartMeal);
                }

            // Create order
            var order = new Order()
            {
                UserId = user.UserId,
                CartId = cartId,
                Price = price,
                CreatedAt = DateTime.UtcNow,
                DeliveredAt = TimeSpan.Zero,
                IsAccepted = "pending",
                IsCanceled = 0,
                IsDelivered = 0,
                PaymentOption = "cash"
            };
            await _orderRepository.Add(order);

            // Simulate approve order
            // Will be replaced by dispatcher service
            order.IsAccepted = "yes";
            order.DeliveryAt = DateTime.UtcNow.AddMinutes(45).TimeOfDay;
            ulong orderId = (await _orderRepository.SingleAsync(order => order.UserId == user.UserId)).OrderId;
            bool isSent = _publisher.PublishOrderEmail($"{user.FirstName} {user.LastName}", user.Email, orderId, cartId);

            if (!isSent)
                return new Order();

            return order;
        }
    }
}
