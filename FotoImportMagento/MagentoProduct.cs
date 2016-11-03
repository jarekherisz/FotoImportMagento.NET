using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FotoImportMagento
{
    class MagentoProduct
    {
        public string id;
        public List<MagentoPic> picture = new List<MagentoPic>();

        public MagentoProduct(string id )
        {
            this.id = id;
        }

        public void addPic(string patch, string poss)
        {
            this.picture.Add(new MagentoPic(patch, poss));
        }
    }

    class MagentoPic
    {

        public string path;
        public string poss;

        public MagentoPic(string path, string poss)
        {
            this.path = path;
            this.poss = poss;
        }
    }

}
