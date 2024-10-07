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

            Warehouse warehouse = new Warehouse();

            Shop shop = new Shop(warehouse);

            warehouse.Deliver(iPhone12, 10);
            warehouse.Deliver(iPhone11, 1);

            //Вывод всех товаров на складе с их остатком

            Cart cart = shop.Cart();
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

    class Warehouse
    {
        private List<GoodsPlace> _products = new List<GoodsPlace>();

        public void Deliver(Product product, int number)
        {

        }
    }

    class Shop
    {
        public Shop(Warehouse warehouse)
        {

        }

        public Cart Cart()
        {
            return new Cart();
        }
    }

    class Cart
    {
        public void Add(Product good, int number)
        {

        }

        public Payment Order()
        {
            return new Payment(0f, "");
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
            if (number > 0)
                Number += number;
        }

        public void RemoveProduct(int number)
        {
            if (number > 0)
                Number -= number;
        }
    }
}
