using Sulakore.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogueRipper
{
    public class CatalogItem
    {
        public int Id { get; set; }
        public string Catalogname { get; set; }
        public int sprite_id { get; set; }
        public string SpecialData { get; set; }
        public int CreditCost { get; set; }
        public int SpecialCost { get; set; }
        public int SpecialType { get; set; }
        public bool AllowGift { get; set; }
        public string Badge { get; set; }
        public int Amount { get; set; }
        public bool Limited { get; set; }

        public void init(HMessage message)
        {
            Id = message.ReadInteger();
            Catalogname = message.ReadString();
            message.ReadBoolean();
            CreditCost = message.ReadInteger();
            SpecialCost = message.ReadInteger();
            SpecialType = message.ReadInteger();
            AllowGift = message.ReadBoolean();
            int ccount = message.ReadInteger();
            for (int p = 0; p < ccount; p++)
            {
                string type = message.ReadString();
                if (type == "b")
                {
                    Badge = message.ReadString();
                }
                else
                {
                    sprite_id = message.ReadInteger();
                    SpecialData = message.ReadString();
                    Amount = message.ReadInteger();
                    Limited = message.ReadBoolean();
                    if (Limited)
                    {
                        message.ReadInteger();
                        message.ReadInteger();
                    }
                }
            }
            message.ReadInteger();
            message.ReadBoolean();
            message.ReadBoolean();
            message.ReadString();
        }
    }
}
