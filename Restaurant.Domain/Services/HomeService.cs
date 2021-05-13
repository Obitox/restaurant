using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapsterMapper;
using Restaurant.DAL.MySQL.Repository;
using Restaurant.Domain.ApiModels;

namespace Restaurant.Domain.Services
{
    public class HomeService : IHomeService
    {
        private readonly ItemRepository _itemRepository;
        private readonly CategoryPortionRepository _categoryPortionRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly MealRepository _mealRepository;
        private readonly IngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;

        public HomeService(IMapper mapper, ItemRepository itemRepository, CategoryPortionRepository categoryPortionRepository, CategoryRepository categoryRepository, MealRepository mealRepository, IngredientRepository ingredientRepository)
        {
            _mapper = mapper;
            _itemRepository = itemRepository;
            _categoryPortionRepository = categoryPortionRepository;
            _categoryRepository = categoryRepository;
            _mealRepository = mealRepository;
            _ingredientRepository = ingredientRepository;
        }

        public async Task<Home> HomeData()
        {
            var categories = (await _categoryRepository.GetAll())
                .Select(category => _mapper.Map<Category>(category));
            var meals = (await _mealRepository.GetAll(null, meal => meal.Image, meal => meal.ItemMeal))
                .Select(meal => _mapper.Map<Meal>(meal));
            var ingredients = (await _ingredientRepository.GetAll(null, ingredient => ingredient.ItemIngredient))
                .Select(ingredient => _mapper.Map<Ingredient>(ingredient));

            var items = await _itemRepository.GetAll();
            var categoryPortions = await _categoryPortionRepository.GetAll(null, categoryPortion => categoryPortion.Portion);

            var listOfApiItems = new List<Item>();
            foreach (var item in items)
            {
                var apiItem = new Item(item.ItemId, item.Title, item.CategoryId, new List<Portion>());
                foreach (var categoryPortion in categoryPortions.Where(categoryPortion => item.CategoryId == categoryPortion.CategoryId))
                {
                    apiItem.Portions.Add(new Portion()
                    {
                        Id = categoryPortion.PortionId,
                        Title = categoryPortion.Portion.Title,
                        Price = item.Price * categoryPortion.Portion.PriceMultiplier,
                        CalorieCount = item.CalorieCount * categoryPortion.Portion.MassCalorieMultiplier,
                        Mass = item.Mass * categoryPortion.Portion.MassCalorieMultiplier
                    });
                }
                listOfApiItems.Add(apiItem);
            } 

            return new Home()
            {
                Items = listOfApiItems,
                Categories = categories,
                Meals = meals,
                Ingredients = ingredients
            };
        }
    }
}