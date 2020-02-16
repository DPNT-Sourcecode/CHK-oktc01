using System;
using System.Collections.Generic;
using System.Linq;

namespace BeFaster.App.Solutions.CHK
{
    public static class CheckoutSolution
    {
        public static int ComputePrice(string skus)
        {
            var skusArray = ValidateAndParseInput(skus);
            if (skusArray != null)
            {
                if (skusArray.Any())
                {
                    var result = skusArray.GroupBy(n => n)
                        .Select(c => new KeyValuePair<string, int>(c.Key.ToString(), c.Count()));

                    int totalPrice = 0;

                    foreach (var p in result)
                    {
                        totalPrice = totalPrice + CalculatePriceForProduct(p.Key, p.Value);
                    }

                    return totalPrice;
                }
            }
            return -1;
        }


        private static char[] ValidateAndParseInput(string skus)
        {
            if (SkusParamIsValid(skus))
            {
                char[] skusArray;
                skusArray = skus.ToLowerInvariant().ToCharArray();

                if (ProductsExistsInPricelist(skusArray))
                    return skusArray;
            }

            return null;
        }

        private static bool ProductsExistsInPricelist(char[] skus)
        {
            var priceList = GetPriceList();
            return
                skus.All(x => priceList.Any(p => p.Key.IndexOf(x.ToString(),StringComparison.CurrentCultureIgnoreCase) > -1));
        }

        private static bool SkusParamIsValid(string param)
        {
            if (string.IsNullOrWhiteSpace(param))
                return false;

            return true;
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
                {"a",50},
                {"3a",130},
                {"b",30},
                {"2b",45},
                {"c",20},
                {"d",15}
            };
        }

    }
}
