using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using ProjectTurtle.src.util;

//later will add in cooldown reduction
namespace ProjectTurtle.src.util {
    class Cooldown {
        internal static Cooldown nullCD = new Cooldown(-10000);
        private double onCD;//when the ability goes on cd
        private float totalCD;//total cooldown in seconds
        private double percentCDreduc;
        private bool startOffCD;
        
        //length in seconds
        public Cooldown(float cdLength){
            totalCD = cdLength;
        }
        public Cooldown(float cdLength, bool startOffCD) : this(cdLength){
            this.startOffCD = startOffCD;
        }
        //returns a boolean with if its ready or not
        public bool ready(double theGameTime) {
            return theGameTime / 1000 >= onCD + getCooldown();
        }
        public bool use(double theGameTime) {
            if (onCD == 0 && !startOffCD)
                onCD = theGameTime / 1000;

            if (theGameTime/1000 >= onCD + getCooldown()) {
                onCD = theGameTime / 1000;
                return true;
            }
            return false;
        }
        public double getTimeLeft(double theGameTime) {
            if (totalCD == -10000) return -10000;
            if ((onCD + totalCD) - theGameTime / 1000 > 0)
                return (onCD + getCooldown()) - theGameTime / 1000;
            return 0;
        }
        public double getFormattedTimeLeft(double theGameTime, int keep) {
            return Math.Round(getTimeLeft(theGameTime),keep);
        }
        public void reset(double theGameTime) {
            onCD = -getCooldown();//why does this work?
        }
        public void addSeconds(float seconds) {
            onCD = onCD + seconds ;
        }
        //returns if it just put it back on cooldown
        public bool pushback(double gameTime, float amt) {
            bool offCd = use(gameTime);
            //then add stuff both times
            addSeconds(amt);
            return offCd;
        }
        public double getCooldown() {
            return totalCD - totalCD*percentCDreduc;
        }
        public void setCDReduc(double percent) {
            percentCDreduc = percent;
        }
    }
}
