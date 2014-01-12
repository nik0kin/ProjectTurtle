using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Audio;
using SFML.Window;
using SFML.Graphics;

namespace ProjectTurtle.src.bossfights {
	internal enum BossFightID {
        none, buseyBoss, tomHanks, dpsCheck, fraiser,
		//forest
		urisdor
    }
    class BossFightLibrary {
        internal const int BOSSES = 5;

        internal static BossFight getBossFight(BossFightID fight) {
            switch (fight) {
            case BossFightID.buseyBoss:
                return new BuseyBossFight();
            case BossFightID.tomHanks:
                return new EvilTomHanksFight();
            case BossFightID.dpsCheck:
                return new CheckDPSBossFight();
            case BossFightID.fraiser:
                return new EvilFraiserFight();
            case BossFightID.urisdor:
                return new UrsidorBossFight();
            }

            return null;
        }
		internal static BossFightInfo getBossInfo(BossFightID fight){
			switch(fight){
			case BossFightID.urisdor:
				return UrsidorBossFight.getBossInfoS();
			default:
				throw new Exception("unsupported fight?: "+fight);
			}
			
		}
		
		//TODO the stuff below this will be useless once getBossInfo is implemented
        internal static BossFightRecord getBossFightRecord(BossFightID fight) {
            return getBossInfo(fight).records;
            /*switch (fight) {
            case BossFightID.buseyBoss:
                return BuseyBossFight.getBossFightRecord();
            case BossFightID.tomHanks:
                return EvilTomHanksFight.getBossFightRecord();
            case BossFightID.dpsCheck:
                return CheckDPSBossFight.getBossFightRecord();
            case BossFightID.fraiser:
                return EvilFraiserFight.getBossFightRecord();
                cas
            }

            return new BossFightRecord();*/
        }
        internal static string getBossFightName(BossFightID fight) {
            switch (fight) {
            case BossFightID.buseyBoss:
                return BuseyBossFight.getName();
            case BossFightID.tomHanks:
                return EvilTomHanksFight.getName();
            case BossFightID.dpsCheck:
                return CheckDPSBossFight.getName();
            case BossFightID.fraiser:
                return EvilFraiserFight.getName();
            }

            return "not found";
        }
        internal static string getBossFightDesc(BossFightID fight) {
            switch (fight) {
            case BossFightID.buseyBoss:
                return BuseyBossFight.getFightDescr();
            case BossFightID.tomHanks:
                return EvilTomHanksFight.getFightDescr();
            case BossFightID.dpsCheck:
                return CheckDPSBossFight.getFightDescr();
            case BossFightID.fraiser:
                return EvilFraiserFight.getFightDescr();
            }

            return "not found_desc";
        }
    }
}
