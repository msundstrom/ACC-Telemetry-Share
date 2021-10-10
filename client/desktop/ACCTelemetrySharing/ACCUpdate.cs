using AssettoCorsaSharedMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ACCTelemetrySharing
{
    struct UpdateFactory
    {
        public static RealTimeUpdate createRealTimeUpdate(
            string shortName,
            Graphics graphics, 
            Physics physics, 
            StaticInfo staticInfo
            )
        {
            return new RealTimeUpdate()
            {
                shortName = shortName,
                iCurrentTime = graphics.iCurrentTime,
                iLastTime = graphics.iLastTime,
                iBestTime = graphics.iBestTime,
                sessionTimeLeft = graphics.sessionTimeLeft,
                iEstimatedLapTime = graphics.iEstimatedLapTime,
                isDeltaPositive = graphics.isDeltaPositive,
                isValidLap = graphics.isValidLap,
                DriverStintTotalTimeLeft = graphics.DriverStintTotalTimeLeft,
                DriverStintTimeLeft = graphics.DriverStintTimeLeft,
                GlobalYellow = graphics.GlobalYellow,
                GlobalYellow1 = graphics.GlobalYellow1,
                GlobalYellow2 = graphics.GlobalYellow2,
                GlobalYellow3 = graphics.GlobalYellow3,
                GlobalWhite = graphics.GlobalWhite,
                GlobalGreen = graphics.GlobalGreen,
                GlobalChequered = graphics.GlobalChequered,
                GlobalRed = graphics.GlobalRed,
                fuelEstimatedLaps = graphics.fuelEstimatedLaps,
                Fuel = physics.Fuel,
                fuelXLap = graphics.fuelXLap,
                Gas = physics.Gas,
                Brake = physics.Brake,
                Clutch = physics.Clutch,
                Gear = physics.Gear,
                TyreCompound = graphics.TyreCompound,
                BrakeTemp = physics.BrakeTemp,
                tyreTemp = physics.tyreTemp,
                padLife = physics.padLife,
                discLife = physics.discLife,
                trackGripStatus = graphics.trackGripStatus,
                rainIntensity = graphics.rainIntensity,
                rainIntensityIn10min = graphics.rainIntensityIn10min,
                rainIntensityIn30min = graphics.rainIntensityIn30min,
                TC = graphics.TC,
                TCCut = graphics.TCCut,
                Abs = graphics.ABS,
                BrakeBias = physics.BrakeBias,
                lightsStage = graphics.lightsStage,
                directionLightsLeft = graphics.directionLightsLeft,
                directionLightsRight = graphics.directionLightsRight,
                mfdTyreSet = graphics.mfdTyreSet,
                mfdFuelToAdd = graphics.mfdFuelToAdd,
                mfdTyrePressureLF = graphics.mfdTyrePressureLF,
                mfdTyrePressureRF = graphics.mfdTyrePressureRF,
                mfdTyrePressureLR = graphics.mfdTyrePressureLR,
                mfdTyrePressureRR = graphics.mfdTyrePressureRR,
                currentTyreSet = graphics.currentTyreSet,
                strategyTyreSet = graphics.strategyTyreSet,
                CarDamage = physics.CarDamage,
                suspensionDamage = physics.suspensionDamage,
                Rpms = physics.Rpms,
                isInPitLane = graphics.isInPitLane,
                isInPit = graphics.isInPit,
                completedLaps = graphics.completedLaps,
            };
        }

        public static OnNewLapUpdate createNewLapUpdate(
            string shortName,
            LapUpdate lapUpdate,
            StintUpdate stintUpdate
            )
        {
            return new OnNewLapUpdate()
            {
                shortName = shortName,
                averageTyreTemp = lapUpdate.averageTemps().rawData(),
                averageTyrePsi = lapUpdate.averagePressures().rawData(),
                stintAverageLapTimeMs = stintUpdate.averageLapTimeMs(),
                predictedPadWear = stintUpdate.predictedPadLife().rawData(),
            };
        }
    }

    [Serializable]
    class RealTimeUpdate
    {
        // origin
        public string shortName;

        // times
        public int iCurrentTime;
        public int iLastTime;
        public int iBestTime;
        public float sessionTimeLeft;
        public int iEstimatedLapTime;
        public int isDeltaPositive;
        public int isValidLap;
        public int DriverStintTotalTimeLeft;
        public int DriverStintTimeLeft;

        // flags
        public int GlobalYellow;
        public int GlobalYellow1;
        public int GlobalYellow2;
        public int GlobalYellow3;
        public int GlobalWhite;
        public int GlobalGreen;
        public int GlobalChequered;
        public int GlobalRed;
        
        // fuel
        public float fuelEstimatedLaps;
        public float Fuel;
        public float fuelXLap;

        // inputs
        public float Gas;
        public float Brake;
        public float Clutch;
        public int Gear;

        // brakes & tyres
        public String TyreCompound;
        public float[] BrakeTemp;
        public float[] tyreTemp;
        public float[] padLife;
        public float[] discLife;

        // weather & track
        public ACC_TRACK_GRIP_STATUS trackGripStatus;
        public ACC_RAIN_INTENSITY rainIntensity;
        public ACC_RAIN_INTENSITY rainIntensityIn10min;
        public ACC_RAIN_INTENSITY rainIntensityIn30min;

        // electronics
        public int TC;
        public int TCCut;
        public int Abs;
        public float BrakeBias;
        public int lightsStage;
        public int directionLightsLeft;
        public int directionLightsRight;

        // MFD
        public int mfdTyreSet;
        public float mfdFuelToAdd;
        public float mfdTyrePressureLF;
        public float mfdTyrePressureRF;
        public float mfdTyrePressureLR;
        public float mfdTyrePressureRR;
        public int currentTyreSet;
        public int strategyTyreSet;

        // damage
        public float[] CarDamage;
        public float[] suspensionDamage;

        // misc
        public int Rpms;
        public int isInPitLane;
        public int isInPit;
        public int completedLaps;
        //public float[] CarCoordinates;
    }

    [Serializable]
    class OnNewLapUpdate
    {
        public string shortName;
        public float[] averageTyreTemp;
        public float[] averageTyrePsi;
        public int stintAverageLapTimeMs;
        public float[] predictedPadWear;

    }

    //[Serializable]
    //class IntermittantUpdate
    //{
    //    public float TC;
    //    public float Abs;
    //    public float BrakeBias;

    //    public int mfdTyreSet;
    //    public float mfdFuelToAdd;
    //    public float mfdTyrePressureLF;
    //    public float mfdTyrePressureRF;
    //    public float mfdTyrePressureLR;
    //    public float mfdTyrePressureRR;

    //    public int currentTyreSet;
    //    public int strategyTyreSet;
    //}

    //[Serializable]
    //class OnDemandUpdate
    //{
    //    public float TC;
    //    public float Abs;
    //    public float BrakeBias;
    //}
}
