using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTurtle.src.stuff {
    class UnlockedHeroes {
        List<PlayerClassNum> unlockedHeroes;//later will have how each was gotten

        internal UnlockedHeroes(bool allUnlocked) {      
            if (allUnlocked) 
                unlockedHeroes = PlayerClassI.getHeroList();
            else
                unlockedHeroes = new List<PlayerClassNum>();

        }
        //p is trying for that number of unlocks, starting with 1
        internal bool isUnlockedClass(int p) {
            return p <= unlockedHeroes.Count;
        }

        internal PlayerClassNum getUnlockedClass(int p) {
            if (p > unlockedHeroes.Count) return PlayerClassNum.none;
            return unlockedHeroes[p-1];
        }
        internal void addUnlockedClass(PlayerClassNum newClass) {
            unlockedHeroes.Add(newClass);
        }
    }
}
