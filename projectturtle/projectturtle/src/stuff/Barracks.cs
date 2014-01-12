using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;

using ProjectTurtle.src.screens;
using ProjectTurtle.src.objects;
using ProjectTurtle.src.ui;
using ProjectTurtle.src.effects;
using ProjectTurtle.src.stuff;
using ProjectTurtle.src.util;
using ProjectTurtle.src.bossfights;

namespace ProjectTurtle.src.stuff {
    class Barracks {
        const string savefile = "barracks.txt";
        const char SEPERATOR = 'z';
        const string SPAREITEMS = "SPAREITEMS";

        List<ChampItemPair> champs;//
        List<ItemID> spareItems;

        private Barracks(){
            champs = new List<ChampItemPair>();
            spareItems = new List<ItemID>();
        }
        internal void addHero( PlayerClassNum pNum, ItemID item){
            champs.Add(new ChampItemPair(pNum, item));
        }
        internal void addItem(ItemID item) {
            if(item == ItemID.na || item == ItemID.none)
                throw new Exception("item is not a real item");
            spareItems.Add(item);
        }
        internal List<ChampItemPair> getChamps() {
            return champs;
        }
        internal int getTotalChamps() {
            return champs.Count;
        }
        internal int getTotalSpareItems(){
            return spareItems.Count;
        }

        internal void swapItems(int changingChampIndex, ItemID newItem) {
            if (!spareItems.Contains(newItem) ^ newItem == ItemID.none)
                throw new Exception("item not in inventory");

            //take give the champ the item
            ItemID exItem = champs[changingChampIndex].item;
            if(newItem != ItemID.none)
                spareItems.Remove(newItem);
            champs[changingChampIndex] = new ChampItemPair(champs[changingChampIndex].hero, newItem, champs[changingChampIndex].id);
            //than put the other item back in spareItems
            if(exItem != ItemID.none)
                spareItems.Add(exItem);

        }

        internal List<ItemID> getSpareItems() {
            return spareItems;
        }

        //simple quick text file implementation for now
        //each row contains:
        //PlayerClassNum ItemID | in integer form
        internal static Barracks load(){
            string s;
            Barracks b = new Barracks();
            bool added = false;
            try {
                TextReader tr = new StreamReader(savefile);
                
                while ((s = tr.ReadLine()) != null) {
                    string[] strs = s.Split(SEPERATOR);
                    if(strs.Length <= 0) continue;
                    int x, y;
                    if (strs[0] == SPAREITEMS) {
                        for (int i = 1;i<strs.Length ;i++) {
                            ItemID newItem = (ItemID)int.Parse(strs[i]);
                            b.addItem(newItem);
                        }
                    }else if (strs.Length == 2) {
                        x = int.Parse(strs[0]);
                        y = int.Parse(strs[1]);
                        added = true;
                        b.addHero((PlayerClassNum)x, (ItemID)y);
                    }
                }


                tr.Close();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }

            if (!added) 
                b = getNewGameBarracks();

            return b;
        }
        internal static void save(Barracks b){
            TextWriter tw = new StreamWriter(savefile);
            tw.Write(SPAREITEMS);
            foreach(ItemID item in b.spareItems){
                tw.Write(""+SEPERATOR + (int)item);
            }
            tw.WriteLine();
            foreach (ChampItemPair hp in b.champs) {
                string s = ""+(int)hp.hero + SEPERATOR + (int)hp.item;
                tw.WriteLine(s);
            }
            tw.Close();
        }
        static Barracks getNewGameBarracks() {
            Barracks b = new Barracks();
            b.addHero(PlayerClassNum.vang, ItemID.none);
            b.addHero(PlayerClassNum.vang, ItemID.none);

            b.addHero(PlayerClassNum.puri, ItemID.none);
            b.addHero(PlayerClassNum.puri, ItemID.none);
            b.addHero(PlayerClassNum.puri, ItemID.none);

            b.addHero(PlayerClassNum.mage, ItemID.none);
            b.addHero(PlayerClassNum.mage, ItemID.none);
            b.addHero(PlayerClassNum.mage, ItemID.none);

            b.addHero(PlayerClassNum.assa, ItemID.none);
            b.addHero(PlayerClassNum.assa, ItemID.none);
            b.addHero(PlayerClassNum.assa, ItemID.none);

            b.addHero(PlayerClassNum.archer, ItemID.none);
            b.addHero(PlayerClassNum.archer, ItemID.none);
            b.addHero(PlayerClassNum.archer, ItemID.none);

            b.addHero(PlayerClassNum.bard, ItemID.none);
            b.addHero(PlayerClassNum.bard, ItemID.none);

            for (int i=0; i < InGame.randy.Next(10)+5;i++ ) {
                b.addItem(Item.getRandomItem());
            }

            return b;
        }

        
    }
    struct ChampItemPair {
		static int sID = 0;
		public int id;
        public PlayerClassNum hero;
        public ItemID item;

        internal ChampItemPair(PlayerClassNum p, ItemID i) {
            hero = p;
            item = i;
			id = sID++;
            if (sID > 16)
                throw new Exception();
        }
        internal ChampItemPair(PlayerClassNum p, ItemID i, int id) {
            hero = p;
            item = i;
            this.id = id;

        }
    }
}
