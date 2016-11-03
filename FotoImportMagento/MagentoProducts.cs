using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FotoImportMagento
{
    class MagentoProducts
    {
        public static List<MagentoProduct> magentoProducts = new List<MagentoProduct>();

        static public void addMagentoProduct(MagentoProduct newMagentoProduct)
        {
            magentoProducts.Add(newMagentoProduct);
        }

        static public MagentoProduct findMagentoProduct(string id)
        {
            foreach (MagentoProduct magentoProduct in magentoProducts)
            {
                if (magentoProduct.id == id)
                {
                    return magentoProduct;
                }
            }
            return null;
        }
    }
}
