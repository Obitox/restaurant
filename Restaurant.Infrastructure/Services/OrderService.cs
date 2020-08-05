using Org.BouncyCastle.Asn1.Cms;
using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository, ICartItemRepository cartItemRepository, ICartMealRepository cartMealRepository)
        {
            _orderRepository = orderRepository ?? throw new NotImplementedException("Order repository");
            _cartRepository = cartRepository ?? throw new NotImplementedException("Cart repository");
            _cartItemRepository = cartItemRepository ?? throw new NotImplementedException("Cart item repository");
            _cartMealRepository = cartMealRepository ?? throw new NotImplementedException("Cart meal repository");
        }

        public async Task<Order> Process(ulong userId, int paymentMethod, List<Item> items, List<Meal> meals, decimal price)
        {
            // Create cart
            var cart = new Cart()
            {
                UserId = userId
            };
            await _cartRepository.Add(cart);

            // Fetch newly created id of cart
            ulong cartId = (await _cartRepository.SingleAsync(cart => cart.UserId == userId)).CartId;

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
                UserId = userId,
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

            return order;
        }
    }
}
