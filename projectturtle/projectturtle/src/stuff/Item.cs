using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.screens;

namespace ProjectTurtle.src.stuff {
    internal struct Item {
        internal const int ITEMS_NUM = 6;
        internal const int ICON_WIDTH = 25;
        //make this way better later
        static Texture glovesTexture, maceTexture, bladeTexture, bowTexture, staffTexture, blendTexture, noneTexture;
        const string path = "images/items/icons/item";//this will change

        internal string name;
        //Buff buff;
        internal string description;
        //internal ItemID self;

        internal Item(ItemID item) {
            name = getItemName(item);;
            //Buff b;
            string d;

            switch(item){
            case ItemID.blade:
                //TODO add buff
                d = "assassin wep";
                break;
            case ItemID.bow:
                d = "archer wep";
                break;
            case ItemID.gloves:
                d = "pyro wep";
                break;
            case ItemID.mace:
                d = "vang wep";
                break;
            case ItemID.staff:
                d = "puri wep";
                break;
            case ItemID.blend:
                d = "bard wep";
                break;
            default:
                throw new Exception("no item?");
            }
            //buff = b;
            description = d;
        }
        internal static void LoadContent() {
            noneTexture = GameBox.loadTexture(path + "none");

            glovesTexture = GameBox.loadTexture(path + "enhancedgloves");
            staffTexture = GameBox.loadTexture(path + "focusedstaff");
            bladeTexture = GameBox.loadTexture(path + "silentblade");
            maceTexture = GameBox.loadTexture(path + "robustmace");
            bowTexture = GameBox.loadTexture(path + "sharpenedbow");
            blendTexture = GameBox.loadTexture(path + "specialblend");
        }
        internal static Texture getItemTexture(ItemID item){
            switch(item){
                case ItemID.blade:
                    return bladeTexture;
                case ItemID.bow:
                    return bowTexture;
                case ItemID.gloves:
                    return glovesTexture;
                case ItemID.mace:
                    return maceTexture;
                case ItemID.staff:
                    return staffTexture;
                case ItemID.blend:
                    return blendTexture;
                case ItemID.none:
                    return noneTexture;
            default:
                throw new Exception("no item?");
            }
        }
        internal static String getItemName(ItemID item){
            switch(item){
                case ItemID.blade:
                    return "Silent Blade";
                case ItemID.bow:
                    return "Sharpened Bow";
                case ItemID.gloves:
                    return "Enhanced Gloves";
                case ItemID.mace:
                    return "Robust Mace";
                case ItemID.staff:
                    return "Focused Staff";
                case ItemID.blend:
                    return "Special Blend";
                case ItemID.none:
                    return "";
            }
            return "unknown";
        }

        internal static ItemID getRandomItem(){
            int r = InGame.randy.Next(ITEMS_NUM);
            return (ItemID)(2 + r);
        }
    }
    internal enum ItemID{
        na, none, 
        blade, mace, staff, bow, blend, gloves
    }
}
