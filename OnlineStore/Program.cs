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

            Warehouse warehouse = new Warehouse();

            Shop shop = new Shop(warehouse);

            warehouse.Deliver(iPhone12, 10);
            warehouse.Deliver(iPhone11, 1);

            warehouse.ShowAllProducts(); //Вывод всех товаров на складе с их остатком

            Cart cart = shop.CreateCart();
            cart.Add(iPhone12, 4);
            cart.Add(iPhone11, 3); //при такой ситуации возникает ошибка так, как нет нужного количества товара на складе

            //Вывод всех товаров в корзине

            Console.WriteLine(cart.Order().Paylink);

            cart.Add(iPhone12, 9); //Ошибка, после заказа со склада убираются заказанные товары

            Console.ReadLine();
        }
    }

    interface IWarehouse
    {
        void GiveOutProduct(Product product, int count);
        int GetCountOfProduct(Product product);
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
        private const int FailedSearchIndex = -1;

        protected List<Cell> Cells { get; } = new List<Cell>();

        public void ShowAllProducts()
        {
            foreach (Cell cell in Cells)
                Console.WriteLine($"{cell.Product.Name} - {cell.Count} шт.");

            Console.WriteLine();
        }

        public virtual void Add(Product product, int count)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            int index = GetCellIndex(product);

            if (index != FailedSearchIndex)
                Cells[index].AddProduct(count);
            else
                Cells.Add(new Cell(product, count));
        }

        public void GiveOutProduct(Product product, int count)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            int index = GetCellIndex(product);

            if (index == FailedSearchIndex)
                throw new ArgumentException(nameof(product));

            int correctCount = Cells[index].Count;

            if (count > correctCount)
                throw new ArgumentOutOfRangeException(nameof(count));

            Cells[index].ReduceQuantity(count);

            if (Cells[index].Count == 0)
                Cells.RemoveAt(index);
        }

        public int GetCountOfProduct(Product product)
        {
            int count = 0;

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            int index = GetCellIndex(product);

            if (index > FailedSearchIndex)
                count = Cells[index].Count;

            return count;
        }

        private int GetCellIndex(Product product)
        {
            int index = -1;

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
        public void Deliver(Product product, int count)
        {
            Add(product, count);
        }
    }

    class Cart : Storage
    {
        private readonly IWarehouse _warehouse;

        public Cart(IWarehouse warehouse)
        {
            _warehouse = warehouse ?? throw new ArgumentNullException(nameof(warehouse));
        }

        public Payment Order()
        {
            EnsureAllProductsQuantity();

            float totalSum = CalculateTotalSum();

            PickUpPurchasedGoodsFromWarehouse();

            Clear();

            return new Payment(totalSum, $"Оплата заказа на сумму - {totalSum} р.\n");
        }

        public override void Add(Product product, int count)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            int countInWarehouse = _warehouse.GetCountOfProduct(product);
            int currentCount = GetCountOfProduct(product);

            if (currentCount + count > countInWarehouse)
                throw new ArgumentException(nameof(count));

            base.Add(product, count);
        }

        public void Clear()
        {
            Cells.Clear();
        }

        private void EnsureAllProductsQuantity()
        {
            foreach (Cell cell in Cells)
                EnsureProductQuantity(cell.Product, cell.Count);
        }

        private void EnsureProductQuantity(Product product, int requiredCount)
        {
            int currentCount = _warehouse.GetCountOfProduct(product);

            if (currentCount < requiredCount)
                throw new ArgumentOutOfRangeException(nameof(requiredCount));
        }

        private float CalculateTotalSum()
        {
            float sum = 0;

            foreach (Cell cell in Cells)
                sum += cell.Product.Price * cell.Count;

            return sum;
        }

        private void PickUpPurchasedGoodsFromWarehouse()
        {
            foreach (Cell cell in Cells)
                _warehouse.GiveOutProduct(cell.Product, cell.Count);
        }
    }

    class Shop
    {
        public Shop(Warehouse warehouse)
        {
            Warehouse = warehouse ?? throw new ArgumentNullException(nameof(warehouse));
        }

        public Warehouse Warehouse { get; }

        public Cart CreateCart()
        {
            return new Cart(Warehouse);
        }
    }

    class Payment
    {
        public Payment(float sum, string payLink)
        {
            if (sum < 0f)
                throw new ArgumentOutOfRangeException(nameof(sum));

            Sum = sum;
            Paylink = payLink;
        }

        public float Sum { get; private set; }
        public string Paylink { get; private set; }
    }

    class Cell
    {
        public Cell(Product products, int count)
        {
            Product = products ?? throw new ArgumentNullException(nameof(products));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            Count = count;
        }

        public Product Product { get; private set; }
        public int Count { get; private set; }

        public void AddProduct(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            Count += count;
        }

        public void ReduceQuantity(int count)
        {
            if (count <= 0 && count > Count)
                throw new ArgumentOutOfRangeException(nameof(count));

            Count -= count;
        }
    }
}
