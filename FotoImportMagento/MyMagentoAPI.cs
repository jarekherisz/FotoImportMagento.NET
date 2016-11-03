using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FotoImportMagento
{
    class MyMagentoAPI : MagentoAPI.MagentoService
    {
        String magentoSessionId;

        public bool Connect()
        {
            try
            {
                this.Url = FotoImportMagento.getInstance().config.textBoxMagentoUrl.Text;
                magentoSessionId = this.login(FotoImportMagento.getInstance().config.textBoxMagentoUser.Text, FotoImportMagento.getInstance().config.textBoxMagentoKey.Text);
                return true;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        public void delAllImage(string sku)
        {
            foreach (MagentoAPI.catalogProductImageEntity image in this.catalogProductAttributeMediaList(this.magentoSessionId, sku, ""))
            {
                this.catalogProductAttributeMediaRemove(this.magentoSessionId, sku, image.file);
            }
        }

        public void addNewImage(string sku, MagentoPic picture)
        {
            MagentoAPI.catalogProductImageFileEntity imageFileEntity = new MagentoAPI.catalogProductImageFileEntity();
            imageFileEntity.content = Serialize(picture.path);
            imageFileEntity.mime = "image/jpeg";

            MagentoAPI.catalogProductAttributeMediaCreateEntity imageCreateEntity =
                new MagentoAPI.catalogProductAttributeMediaCreateEntity();
            imageCreateEntity.file = imageFileEntity;
            imageCreateEntity.exclude = "0";
            imageCreateEntity.label = "";
            imageCreateEntity.position = picture.poss;
            if(picture.poss == "0")
                imageCreateEntity.types = new string[] {"image", "small_image", "thumbnail"};


            this.catalogProductAttributeMediaCreate(this.magentoSessionId, sku, imageCreateEntity, "");
        }

        public static string Serialize(string fileName)
        {
            using (FileStream reader = new FileStream(fileName, FileMode.Open))
            {
                byte[] buffer = new byte[reader.Length];
                reader.Read(buffer, 0, (int)reader.Length);
                return Convert.ToBase64String(buffer);
            }
        }
    }
}
