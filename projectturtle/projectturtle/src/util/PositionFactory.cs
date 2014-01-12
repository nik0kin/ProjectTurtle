using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

using System.Collections;

using ProjectTurtle.src.objects;
using ProjectTurtle.src.ui;
using ProjectTurtle.src.effects;
using ProjectTurtle.src.util;

namespace ProjectTurtle.src.util {
    class PositionFactory {
        internal static Hashtable getRaidSetup(List<Moveable> list) {
            Hashtable raidSetup = new Hashtable();//key = name, value = vector2?
            foreach (Moveable m in list) {
                try {
                    raidSetup.Add(m.getName(), m.getPosition());
                } catch (Exception e) { Console.WriteLine(e + " "); }
            }
            return raidSetup;
        }

        internal static void getSetupRaid(List<Moveable> raid, Hashtable raidSetup) {
            if (raidSetup == null) return;
            foreach (Moveable m in raid) {
                Console.Write(m.getName()+ " "+m.getPosition() + " -> ");
                Object pos = raidSetup[m.getName()];
                if (pos == null) continue;
                m.setPosition((Vector2f)pos);
                Console.WriteLine(m.getPosition());
            }
        }
    }
}
