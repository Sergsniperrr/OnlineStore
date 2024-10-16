using System;
using System.Collections.Generic;

namespace OnlineStore
{
    class Program
    {
        static void Main(string[] args)
        {
            Product iPhone12 = new Product("IPhone 12", 20000f);
            Product iPhone11 = new Product("IPhone 11", 15000f);

            Shop shop = new Shop();

            shop.Warehouse.Deliver(iPhone12, 10);
            shop.Warehouse.Deliver(iPhone11, 1);

            shop.Warehouse.ShowAllProducts(); //Вывод всех товаров на складе с их остатком

            shop.Cart.Add(iPhone12, 4);
            shop.Cart.Add(iPhone11, 3); //при такой ситуации возникает ошибка так, как нет нужного количества товара на складе

            //Вывод всех товаров в корзине

            Console.WriteLine(shop.Cart.Order().Paylink);

            shop.Cart.Add(iPhone12, 9); //Ошибка, после заказа со склада убираются заказанные товары

            Console.ReadLine();
        }
    }

    interface IWarehouse
    {
        void GiveOutProduct(Product product, int number);
        int GetNumberOfProduct(Product product);
    }

    class Product
    {
        public Product(string name, float price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; private set; }
        public float Price { get; private set; }
    }

    class Storage : IWarehouse
    {
        protected const int FailedSearchIndex = -1;

        protected List<GoodsPlace> Spots = new List<GoodsPlace>();

        public void ShowAllProducts()
        {
            foreach (GoodsPlace spot in Spots)
                Console.WriteLine($"{spot.Product.Name} - {spot.Number} шт.");

            Console.WriteLine();
        }

        public virtual void Add(Product product, int number)
        {
            int index = GetSpotIndex(product);

            if (index != FailedSearchIndex)
                Spots[index].AddProduct(number);
            else
                Spots.Add(new GoodsPlace(product, number));
        }

        public void GiveOutProduct(Product product, int number)
        {
            int correctNumber;
            int index = GetSpotIndex(product);

            if (index == FailedSearchIndex)
                throw new ArgumentException(nameof(product));

            correctNumber = Spots[index].Number;

            if (number > correctNumber)
                throw new ArgumentOutOfRangeException(nameof(number));

            Spots[index].ReduceQuantity(number);

            if (Spots[index].Number == 0)
                Spots.RemoveAt(index);
        }

        public int GetNumberOfProduct(Product product)
        {
            int count = 0;
            int index = GetSpotIndex(product);

            if (index > FailedSearchIndex)
                count = Spots[index].Number;

            return count;
        }

        private int GetSpotIndex(Product product)
        {
            int index = -1;

            if (Spots.Count == 0)
                return index;

            for (int i = 0; i < Spots.Count; i++)
            {
                if (Spots[i].Product.Name.ToLower() == product.Name.ToLower())
                {
                    index = i;
                    break;
                }
            }

            return index;
        }
    }

    class Warehouse : Storage
    {
        public void Deliver(Product product, int number)
        {
            Add(product, number);
        }
    }

    class Cart : Storage
    {
        private IWarehouse _warehouse;

        public Cart(IWarehouse warehouse)
        {
            _warehouse = warehouse;
        }

        public Payment Order()
        {
            Payment payment;
            float zeroValue = 0f;
            float totalSum;

            if (CheckRequiredQuantityOfAllProducts())
            {
                totalSum = CalculateTotalSum();
                payment = new Payment(totalSum, $"Оплата заказа на сумму - {totalSum} р.\n");
                PickUpPurchasedGoodsFromWarehouse();
                Clear();
            }
            else
            {
                Console.WriteLine("Невозможно оплатить заказ! Один или несколько товаров отсутствуют на складе!\n");
                payment = new Payment(zeroValue, "");
            }

            return payment;
        }

        public override void Add(Product product, int number)
        {
            int currentNumber = _warehouse.GetNumberOfProduct(product);

            if (number > currentNumber)
            {
                Console.WriteLine($"Невозможно добавить в корзину \"{product.Name}\" - {number} шт., т.к.\n" +
                                  $"в наличии на складе имеется только {currentNumber} шт.!\n");

                return;
            }

            base.Add(product, number);

            Console.WriteLine($"\"{product.Name}\" в кол-ве {number} шт. успешно добавлен в корзину.\n");
        }

        public void Clear()
        {
            Spots.Clear();
        }

        private bool CheckRequiredQuantityOfAllProducts()
        {
            bool isEnoughQuantityOfAllProducts = true;

            if (Spots.Count == 0)
                return false;

            foreach (GoodsPlace cell in Spots)
            {
                if (CheckRequiredOfProductQuantity(cell.Product, cell.Number) == false)
                {
                    Console.WriteLine($"За время оформления заказа количество товара " +
                                      $"\"{cell.Product.Name}\" на складе изменилось!");

                    isEnoughQuantityOfAllProducts = false;
                    break;
                }
            }

            return isEnoughQuantityOfAllProducts;
        }

        private bool CheckRequiredOfProductQuantity(Product product, int requiredNumber)
        {
            int currentNumber = _warehouse.GetNumberOfProduct(product);
            bool result = currentNumber >= requiredNumber;

            if (result == false)
                throw new ArgumentOutOfRangeException(nameof(requiredNumber));

            return result;
        }

        private float CalculateTotalSum()
        {
            float sum = 0;

            foreach (GoodsPlace cell in Spots)
                sum += cell.Product.Price * cell.Number;

            return sum;
        }

        private void PickUpPurchasedGoodsFromWarehouse()
        {
            foreach (GoodsPlace spot in Spots)
                _warehouse.GiveOutProduct(spot.Product, spot.Number);
        }
    }

    class Shop
    {
        public readonly Warehouse Warehouse = new Warehouse();
        public readonly Cart Cart;

        public Shop()
        {
            Cart = new Cart(Warehouse);
        }
    }

    class Payment
    {
        public Payment(float sum, string payLink)
        {
            Sum = sum;
            Paylink = payLink;
        }

        public float Sum { get; private set; }
        public string Paylink { get; private set; }
    }

    class GoodsPlace
    {
        public GoodsPlace(Product products, int number)
        {
            Product = products;

            if (number < 0)
                Number = 0;
            else
                Number = number;
        }

        public Product Product { get; private set; }
        public int Number { get; private set; }

        public void AddProduct(int number)
        {
            if (number <= 0)
                throw new ArgumentOutOfRangeException(nameof(number));

            Number += number;
        }

        public void ReduceQuantity(int number)
        {
            if (number <= 0)
                throw new ArgumentOutOfRangeException(nameof(number));

            Number -= number;
        }
    }
}
