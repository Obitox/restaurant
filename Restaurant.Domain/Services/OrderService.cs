using Restaurant.Domain.RabbitMQ;
using Restaurant.Domain.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Restaurant.DAL.MySQL.Models;
using Restaurant.DAL.MySQL.Repository;

namespace Restaurant.Domain.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrderRepository _orderRepository;
        private readonly CartRepository _cartRepository;
        private readonly CartItemRepository _cartItemRepository;
        private readonly CartMealRepository _cartMealRepository;
        private readonly IPublisher _publisher;
        private readonly UserRepository _userRepository;

        public OrderService(OrderRepository orderRepository, CartRepository cartRepository, CartItemRepository cartItemRepository, CartMealRepository cartMealRepository, IPublisher publisher, UserRepository userRepository)
        {
            _orderRepository = orderRepository ?? throw new NotImplementedException("Order repository instance not implemented");
            _cartRepository = cartRepository ?? throw new NotImplementedException("Cart repository instance not implemented");
            _cartItemRepository = cartItemRepository ?? throw new NotImplementedException("Cart item repository not implemented");
            _cartMealRepository = cartMealRepository ?? throw new NotImplementedException("Cart meal repository not implemented");
            _publisher = publisher ?? throw new NotImplementedException("Publisher instance not implemented");
            _userRepository = userRepository ?? throw new NotImplementedException("User repository instance not implemented");
        }

        public async Task<OrderView> Process(ulong userId, int paymentMethod, List<Item> items, List<Meal> meals, decimal price)
        {
            var user = await _userRepository.SingleAsync(user => user.UserId == userId);
            if (user == null)
                return new OrderView();

            // Create cart
            var cart = new Cart()
            {
                UserId = user.UserId
            };
            await _cartRepository.Add(cart);

            // Fetch newly created id of cart
            var cartId = (await _cartRepository.SingleAsync(cart => cart.UserId == user.UserId)).CartId;

            // Create cart-item junctions
            if (items.Count > 0)
                foreach (var cartItem in items.Select(item => item.CartItem.ElementAt(0)))
                {
                    cartItem.CartId = cartId;
                    await _cartItemRepository.Add(cartItem);
                }

            // Create cart-meal junctions
            if (meals.Count > 0)
                foreach (var cartMeal in meals.Select(meal => meal.CartMeal.ElementAt(0)))
                {
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
                DeliveryAt = DateTime.MinValue,
                IsAccepted = "pending",
                IsCanceled = 0,
                IsDelivered = 0,
                PaymentOption = "cash"
            };
            await _orderRepository.Add(order);

            // Simulate approve order
            // Will be replaced by dispatcher service
            order.IsAccepted = "yes";
            order.DeliveryAt = order.CreatedAt.AddMinutes(45);
            // var orderId = (await _orderRepository.SingleAsync(order => order.UserId == user.UserId)).OrderId;
// .Where(cartItem => cartItem.CartId == cartId)
//                 .Include(cartItem => cartItem.Item)
//                 .Include(cartItem => cartItem.Portion)

            var cartItems = await _cartItemRepository.GetAll(
                cartItem => cartItem.CartId == cartId, 
                cartItem => cartItem.Item, cartItem => cartItem.Portion
            );

            var orderItems = cartItems.Select(cartItem => 
                new OrderItem() 
                {
                    Title = cartItem.Item.Title, 
                    PersonalPreference = cartItem.PersonalPreference, 
                    PortionTitle = cartItem.Portion.Title, 
                    Amount = cartItem.Amount,
                    Price = Math.Round(cartItem.Item.Price * cartItem.Portion.PriceMultiplier * cartItem.Amount, 2, MidpointRounding.ToEven)
                }).ToList();

            var cartMeals = await _cartMealRepository.GetAll(
                cartMeal => cartMeal.CartId == cartId,
                cartMeal => cartMeal.Meal
            );

            var orderMeals = cartMeals.Select(cartMeal =>
                new OrderMeal()
                {
                    Title = cartMeal.Meal.Title,
                    PersonalPreference = cartMeal.PersonalPreference,
                    Amount = cartMeal.Amount,
                    Price = cartMeal.Meal.Price * cartMeal.Amount
                }).ToList();
            
            var orderEmailView = new OrderView()
            {
                OrderId = order.OrderId,
                Price = order.Price,
                PaymentOption = order.PaymentOption,
                // TODO: Fetch dispatcher input
                DeliveryAt = order.DeliveryAt,
                CreatedAt = order.CreatedAt,
                OrderItems = orderItems,
                OrderMeals = orderMeals
            };

            var isSent = _publisher.PublishOrderEmail(user.UserId, $"{user.FirstName} {user.LastName}", user.Email, orderEmailView);

            return !isSent ? new OrderView() : orderEmailView;
        }

        public async Task<object> OrderEmailView(ulong cartId, ulong orderId)
        {
            var cartItems = await _cartItemRepository.GetAll(cartItem => cartItem.CartId == cartId);
            var order = await _orderRepository.SingleAsync(order => order.OrderId == orderId);

            var items = cartItems.Select(cartItem => 
                new OrderItem()
                {
                    Title = cartItem.Item.Title,
                    PersonalPreference = cartItem.PersonalPreference,
                    PortionTitle = cartItem.Portion.Title,
                    Amount = cartItem.Amount
                }).ToList();

            var orderEmailView = new OrderView()
            {
                OrderId = order.OrderId,
                Price = order.Price,
                PaymentOption = order.PaymentOption,
                // TODO: Fetch dispatcher input
                DeliveryAt = order.DeliveryAt,
                OrderItems = items
            };

            return orderEmailView;
        }
    }
}
