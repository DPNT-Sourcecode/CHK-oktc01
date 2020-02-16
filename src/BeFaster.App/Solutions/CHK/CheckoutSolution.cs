using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace BeFaster.App.Solutions.CHK
{
    public static class CheckoutSolution
    {

        private class Product
        {
            public string Name { get; set; }
            public int Count { get; set; }

            public Product(string name, int count)
            {
                Name = name;
                Count = count;
            }
        }

        private class FreeProduct: Product
        {
            public int NumberOfItemsRequired { get; set; }

            public FreeProduct(string name, int numberOfItemsRequired, int count = 1) : base(name, count)
            {
                NumberOfItemsRequired = numberOfItemsRequired;
            }
        }


        /// <summary>
        /// Calculates price.
        /// Sku parameter must be without delimiters and contain the uppercase product codes.
        /// Based on the price list there will be the best offer applied. 
        /// </summary>
        /// <param name="skus"></param>
        /// <returns></returns>
        public static int ComputePrice(string skus)
        {
            var skusArray = ValidateAndParseInput(skus);
            if (skusArray != null)
            {
                var products = skusArray.GroupBy(n => n)
                    .Select(c => new Product(c.Key.ToString(), c.Count())).ToList();

                int totalPrice = 0;

                // Apply free item discount
                products = ApplyFreeItems(products);

                foreach (var p in products)
                {
                    totalPrice = totalPrice + CalculatePriceForProduct(p.Name, p.Count);
                }
                
                return totalPrice;
            }
            return -1;
        }


        private static char[] ValidateAndParseInput(string skus)
        {
            if (skus != null)
            {
                char[] skusArray;
                skusArray = skus.ToCharArray();

                if (SkusValid(skusArray))
                    return skusArray;
            }

            return null;
        }

        private static bool SkusValid(char[] skus)
        {
            if (!skus.Any()) return true;

            var priceList = GetPriceList();
            return
                skus.All(x => priceList.Any(p => p.Key.IndexOf(x) > -1));
        }

        private static int CalculatePriceForProduct(string product, int numberOfItems)
        {
            int result = 0;

            while (numberOfItems > 0)
            {
                var bestPrice = GetBestOffer(product, numberOfItems);
                if (bestPrice.Key == 1)
                {
                    result += bestPrice.Value * numberOfItems;
                    return result;
                }

                result += bestPrice.Value;
                numberOfItems -= bestPrice.Key;
            }

            return result;
        }


        /// <summary>
        /// Returns a best offer for the product and number of items.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="numberOfItems"></param>
        /// <returns></returns>
        private static KeyValuePair<int, int> GetBestOffer(string product, int numberOfItems)
        {
            var res = GetPriceList().Where(p =>
                p.Key.IndexOf(product, StringComparison.CurrentCultureIgnoreCase) > -1)
                .Select(o =>
                new KeyValuePair<int, int>(
                    GetProductCountFromProductCode(product, o.Key), o.Value));

            return res.Where(x => numberOfItems >= x.Key).OrderByDescending(o => o.Key).FirstOrDefault();
        }


        /// <summary>
        /// Retrieves free item offers from GetFreeProductList().
        /// Validates if requirements match, updates product count for total basket calculation.
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        private static List<Product> ApplyFreeItems(List<Product> products)
        {
            // Product names and number of items to remove.
            var freeItemList = new HashSet<FreeProduct>();

            foreach (var pr in products)
            {

                // Full free item offer list for current product
                var offersForProduct = GetFreeProductList().Where(p =>
                        p.Key.IndexOf(pr.Name, StringComparison.CurrentCultureIgnoreCase) > -1)
                    .Select(o =>
                        new FreeProduct(o.Value, GetProductCountFromProductCode(pr.Name, o.Key))).ToList();

                
                // Check if offer available and offered product exists in basket
                if (offersForProduct.Any() && products.Any(x => x.Name == offersForProduct.First().Name))
                {
                    int discountForProductCount = pr.Count;
                    bool sameProductDiscount = pr.Name == offersForProduct.First().Name;
                    var freeItemOffer = offersForProduct.Where(x =>
                        sameProductDiscount ?  
                            discountForProductCount > x.NumberOfItemsRequired : 
                            discountForProductCount >= x.NumberOfItemsRequired
                        ).OrderByDescending(o => o.Count).FirstOrDefault();

                    while (freeItemOffer != null)
                    {
                        freeItemList.Add(new FreeProduct(freeItemOffer.Name, freeItemOffer.Count));
                        discountForProductCount -= freeItemOffer.NumberOfItemsRequired;

                        freeItemOffer = offersForProduct.Where(x =>
                            sameProductDiscount ?
                                discountForProductCount > x.NumberOfItemsRequired :
                                discountForProductCount >= x.NumberOfItemsRequired
                                ).OrderByDescending(o => o.Count).FirstOrDefault();
                    }
                }
            }

            foreach (var f in freeItemList)
            {
                var product = products.Single(p => p.Name == f.Name);
                product.Count -= f.Count;
            }

            return products;
        }




        private static int ApplyFreeItems(int totalPrice, List<KeyValuePair<string,int>> products)
        {
            foreach (var pr in products)
            {
                var res = GetFreeProductList().Where(p =>
                        p.Key.IndexOf(pr.Key, StringComparison.CurrentCultureIgnoreCase) > -1)
                    .Select(o =>
                        new KeyValuePair<int, string>(
                            GetProductCountFromProductCode(pr.Key, o.Key), o.Value)).ToList();

                // Having count with discounted product name

                // Check if offer available and offered product exists in basket
                if (res.Any() && products.Any(x => x.Key == res.First().Value))
                {
                    var freeItemOffer = res.Where(x => pr.Value >= x.Key).OrderByDescending(o => o.Key).FirstOrDefault();

                    // Get price by product code
                    var discount = GetPriceList().SingleOrDefault(x => x.Key == freeItemOffer.Value);

                    // Discount this price
                    totalPrice -= discount.Value;
                }
            }

            return totalPrice;
        }


        private static int GetProductCountFromProductCode(string productName, string productCode)
        {
            productCode = productCode.ToLowerInvariant().Replace(productName.ToLowerInvariant(), "");
            if (string.IsNullOrWhiteSpace(productCode)) return 1;

            return int.Parse(productCode);
        }

        private static Dictionary<string, int> GetPriceList()
        {
            return new Dictionary<string, int>
            {
                {"A",50},
                {"3A",130},
                {"5A",200},
                {"B",30},
                {"2B",45},
                {"C",20},
                {"D",15},
                {"E",40},
                {"F",10},
                {"G",20},
                {"H",10},
                {"5H",45},
                {"10H",80},
                {"I",35},
                {"J",60},
                {"K",80},
                {"2K",150},
                {"L",90},
                {"M",15},
                {"N",40},
                {"O",10},
                {"P",50},
                {"5P",200},
                {"Q",30},
                {"3Q",80},
                {"R",50},
                {"S",30},
                {"T",20},
                {"U",40},
                {"V",50},
                {"2V",90},
                {"3V",130},
                {"W",20},
                {"X",90},
                {"Y",10},
                {"Z",50}
            };
        }

        private static Dictionary<string, string> GetFreeProductList()
        {
            return new Dictionary<string, string>
            {
                {"2E", "B"},
                {"2F", "F"},
                {"3N", "M"},
                {"3R", "Q"},
                {"3U", "U"},
            };
        }

    }
}
