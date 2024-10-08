using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore
{
    class Program
    {
        static void Main(string[] args)
        {
            Product iPhone12 = new Product("IPhone 12");
            Product iPhone11 = new Product("IPhone 11");

            Shop shop = new Shop();

            shop.Warehouse.Deliver(iPhone12, 10);
            shop.Warehouse.Deliver(iPhone11, 1);

            shop.Warehouse.ShowAllProducts(); //Вывод всех товаров на складе с их остатком

            Cart cart = shop.Cart;
            cart.Add(iPhone12, 4);
            cart.Add(iPhone11, 3); //при такой ситуации возникает ошибка так, как нет нужного количества товара на складе

            //Вывод всех товаров в корзине

            Console.WriteLine(cart.Order().Paylink);

            cart.Add(iPhone12, 9); //Ошибка, после заказа со склада убираются заказанные товары
        }
    }

    class Product
    {
        public Product(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }

    class Storage
    {
        protected List<GoodsPlace> Cells = new List<GoodsPlace>();

        public void ShowAllProducts()
        {
            foreach (GoodsPlace place in Cells)
                Console.WriteLine($"{place.Product.Name} - {place.Number} шт.");
        }

        public void Add(Product product, int number)
        {
            int failedSearchIndex = -1;
            int index = SearchProduct(product);

            if (index != failedSearchIndex)
                Cells[index].AddProduct(number);
            else
                Cells.Add(new GoodsPlace(product, number));

        }

        private int SearchProduct(Product product)
        {
            int index = -1;

            if (Cells.Count == 0)
                return index;

            for (int i = 0; i < Cells.Count; i++)
            {
                if (Cells[i].Product.Name.ToLower() == product.Name.ToLower())
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
        public Payment Order()
        {
            return new Payment(0f, "");
        } 
    }

    class Shop
    {
        public readonly Warehouse Warehouse = new Warehouse();
        public readonly Cart Cart = new Cart();


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

        public void RemoveProduct(int number)
        {
            if (number <= 0)
                throw new ArgumentOutOfRangeException(nameof(number));

            Number -= number;
        }
    }
}
