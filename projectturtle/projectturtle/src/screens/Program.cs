using System;

namespace ProjectTurtle.src.screens {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args) {
            GameBox gbox = new GameBox();
            
            gbox.loadContent();

            //while(true){
            gbox.run();
            //}

            //gbox.unloadContent();
        }
    }
}

