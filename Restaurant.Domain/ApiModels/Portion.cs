﻿namespace Restaurant.Domain.ApiModels
{
    public class Portion
    {
        public ulong Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public decimal CalorieCount { get; set; }
        public decimal Mass { get; set; }

        public Portion(ulong id, string title, decimal price, uint calorieCount, uint mass)
        {
            Id = id;
            Title = title;
            Price = price;
            CalorieCount = calorieCount;
            Mass = mass;
        }

        public Portion()
        {
        }    
    }
}