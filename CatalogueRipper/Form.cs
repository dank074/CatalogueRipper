using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sulakore.Communication;
using Sulakore.Modules;
using Sulakore.Protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tangine;

namespace CatalogueRipper
{

    [Module("name " + Form.version, "desc " + Form.version)]
    [Author("Author " + Form.version)]
    public partial class Form : ExtensionForm
    {
        private Page catalog = new Page();
        private Dictionary<int, Page> allPages = new Dictionary<int, Page>();


        private ushort CatalogPageInit => this.Game.Messages.FirstOrDefault(o => o.Key == "599c6043cc855bbeecebb2ead7c7a6f9").Value.FirstOrDefault().Id;
        private ushort CatalogReload => this.Game.Messages.FirstOrDefault(o => o.Key == "43959cefa54e74087557d3afc9472192").Value.FirstOrDefault().Id;
        private ushort GetPage => this.Game.Messages.FirstOrDefault(o => o.Key == "581574d1aae8a2da37cb4b3422f993bd").Value.FirstOrDefault().Id;
        private ushort CatalogPage => this.Game.Messages.FirstOrDefault(o => o.Key == "581574d1aae8a2da37cb4b3422f993bd").Value.FirstOrDefault().Id;


        public Form()
        {
          ConsoleInitializer.Init();
            Console.WriteLine($"Form1 ");
            InitializeComponent();
            Console.WriteLine($"pageinit ");
        }

        public override void HandleOutgoing(DataInterceptedEventArgs e)
        {
            //    Console.WriteLine("OuTOING! > " + e.Packet.Header);
            //
            Sulakore.Habbo.MessageItem message;
            if (this.Game.OutMessages.TryGetValue(e.Packet.Header, out message))
            {
                //  Console.WriteLine(message.Hash);
                switch (message.Hash)
                {
                    case "ba6e5ec5767486804f64557233e71a03":
                        {
                            e.Packet.ReadString();
                            var clickevent = e.Packet.ReadString();
                            Console.WriteLine($"Toolbar clicked : {clickevent}");
                            if (clickevent == "MEMENU")
                            {
                                HMessage reloadEvent = new HMessage(CatalogReload);
                                reloadEvent.WriteBoolean(false);
                                this.Connection.SendToClientAsync(reloadEvent);
                            }
                            break;
                        }

                }
            }
            base.HandleOutgoing(e);
        }
        public override void HandleIncoming(DataInterceptedEventArgs e)
        {
            // Console.WriteLine("Incomming! > " + e.Packet.Header);

            Sulakore.Habbo.MessageItem message;
            if (this.Game.InMessages.TryGetValue(e.Packet.Header, out message))
            {
                //   Console.WriteLine(message.Hash);
                switch (message.Hash)
                {

                    case "599c6043cc855bbeecebb2ead7c7a6f9":
                        {
                            ReadPages(e.Packet);
                            break;
                        }
                    case "8cd57bb7ca6f920991554ea36ab615cf":
                        {
                            Console.WriteLine("Fillpage");
                            Task.Run(() => fillPage(e.Packet));
                            break;
                        }
                }
            }
            base.HandleIncoming(e);
        }

        public async Task fillPage(HMessage message)
        {
            try
            {
                int pageId = message.ReadInteger();
                if (!allPages.ContainsKey(pageId))
                {
                    Console.WriteLine("Page undifined!");
                    return;
                }
                Page page = allPages[pageId];
                if (page.Loaded)
                {
                    Console.WriteLine("page already loaded");
                    return;
                }
                page.Loaded = true;

                message.ReadString();// NORMAL
                page.Layout = message.ReadString();  // page layout

                int imagesCount = message.ReadInteger();
                for (int i = 0; i < imagesCount; i++)
                {
                    page.Images.Add(message.ReadString()); // images
                }

                int textsCount = message.ReadInteger();
                for (int i = 0; i < textsCount; i++)
                {
                    page.Texts.Add(message.ReadString()); // texts
                }
                int OfferDataCount = message.ReadInteger();
                for (int i = 0; i < OfferDataCount; i++)
                {
                    CatalogItem item = new CatalogItem();
                    item.init(message);
                    page.Items.Add(item);

                }
            }
            catch (Exception e)
            {

            }
        }
        public async Task InitPages(Page page)
        {
            Console.WriteLine($"Init page> {page.Name}");

            Console.WriteLine("{l}{u:3213}{i:"+page.Id+"}{i:-1}{s:NORMAL}");
            HMessage message = new HMessage(GetPage);
            message.WriteInteger(page.Id);
            message.WriteInteger(-1);
            message.WriteString("NORMAL");
            await this.Connection.SendToServerAsync(message);
            foreach (Page subPage in page.SubPages)
            {
                await Task.Delay(300);
                await InitPages(subPage);
            }
        }
        public void ReadPages(HMessage message)
        {
            try
            {
                allPages.Clear();
                catalog = ReadPage(message);
                Task.Run(async () => await InitPages(catalog)) ;

                // Console.WriteLine(prettyJson);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public Page ReadPage(HMessage message, int parentId = -2)
        {
            Page page = new Page();
            page.ParentId = parentId; ;
            message.ReadBoolean();
            page.Icon = message.ReadInteger();
            page.Id = message.ReadInteger();
            //   Console.WriteLine($" pageId {page.Id}"); ;
            //   Console.WriteLine($" pageIcon{page.Id}"); ;
            page.Name = message.ReadString();
            page.Caption = message.ReadString();
            // Console.WriteLine($" pageName {page.Name}"); ;
            // Console.WriteLine($" pageCaption {page.Caption}"); ;
            int idkyet = message.ReadInteger();
            //    Console.WriteLine($" SearchableItems {idkyet}"); ;
            for (int i = 0; i < idkyet; i++)
            {
                message.ReadInteger();
            }
            int pagescount = message.ReadInteger();
            //    Console.WriteLine($"pagescount > " + pagescount);
            for (int i = 0; i < pagescount; i++)
            {
                Page p = ReadPage(message, page.Id);
                if (!allPages.ContainsKey(p.Id))
                {
                    allPages.Add(p.Id, p);
                }
                else
                {
                    allPages[p.Id] = p;
                }

                page.SubPages.Add(allPages[p.Id]);
            }

            return page;
        }
        public const string version = "221";

        private void button1_Click(object sender, EventArgs e)
        {
            var pageJson = JsonConvert.SerializeObject(new { p = catalog, all = allPages });
            string prettyJson = JToken.Parse(pageJson).ToString(Formatting.Indented);
            File.WriteAllText("pages.json", prettyJson);
        }


        //public override void HandleIncomming(DataInterceptedEventArgs e)
        //{

        //}
    }
}
