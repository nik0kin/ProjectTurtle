using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using ProjectTurtle.src.objects;

namespace ProjectTurtle.src.stuff {
    class DamageMeter {
        Hashtable damageMeter;//phase 1, (key = name, value = amt)
        bool change;
        string[] oldList;

        internal DamageMeter() {
            damageMeter = new Hashtable();
        }
        //update damage
        internal void addToDamageMeter(Moveable giver, float amount) {
            amount = (float)Math.Round(amount,3);
            if (damageMeter.ContainsKey(giver.getName())) {
                amount = amount + (float)damageMeter[giver.getName()];
            }
            damageMeter.Remove(giver.getName());
            damageMeter.Add(giver.getName(), amount);
            change = true;
        }

        //in order from highest to low
        internal string[] getOrderedDamageStrings(float timeSeconds) {
            if (damageMeter == null) return null;
            if (!change && oldList != null) return oldList;
            string[] list = new string[damageMeter.Count];
            //turn stuff into arrays
            string[] keys = new string[damageMeter.Count];
            double[] values = new double[damageMeter.Count];
            damageMeter.Keys.CopyTo((string[])keys, 0);
            damageMeter.Values.CopyTo((double[])values, 0);
            //sort arrays
            // Sorting: Bubble Sort
            int i = 0, j = 0;
            double t = 0;
            string dummy;
            double[] c = values;
            for (i = 0; i < c.Length; i++) {
                for (j = i + 1; j < c.Length; j++) {
                    if (c[i] < c[j]) {//switch for ascend/decend
                        t = c[i];
                        c[i] = c[j];
                        c[j] = t;
                        dummy = keys[i];
                        keys[i] = keys[j];
                        keys[j] = dummy;
                    }
                }
            }
            //then put them in a list together
            for (i = 0; i < c.Length;i++ ) {
                list[i] = keys[i] + " " + Math.Round(values[i],2) + " ("+Math.Round(values[i]/timeSeconds,2) +")";
            }

            oldList = list;
            change = false;
            return list;
        }
        internal void resetMeters(){
            damageMeter = new Hashtable();
        }
        //TODO really silly? Assassin Archer Bard Puri Mage Vangua
        public static Color getColor(string s) {
            switch (s[3]) {
            case 'a'://Assassin
                return PlayerClassI.getClassColor(PlayerClassNum.assa);
            case 'h'://Archer
                return PlayerClassI.getClassColor(PlayerClassNum.archer);
            case 'd'://Bard
                return PlayerClassI.getClassColor(PlayerClassNum.bard);
            case 'i'://Puri
                return PlayerClassI.getClassColor(PlayerClassNum.puri);
            case 'm'://Flame Mage
                return PlayerClassI.getClassColor(PlayerClassNum.mage);
            case 'g'://Vangua
                return PlayerClassI.getClassColor(PlayerClassNum.vang);
            default:
                return Color.Cyan;
            }
        }

        
    }
}
