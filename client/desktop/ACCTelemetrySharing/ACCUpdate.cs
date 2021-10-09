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
            Graphics graphics, 
            Physics physics, 
            StaticInfo staticInfo
            )
        {
            return new RealTimeUpdate()
            {
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
                Rpms = physics.Rpms,
                isInPitLane = graphics.isInPitLane,
                isInPit = graphics.isInPit,
            };
        }
    }

    [Serializable]
    class RealTimeUpdate
    {
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

        // misc
        public int Rpms;
        public int isInPitLane;
        public int isInPit;
        //public float[] CarCoordinates;
    }

    [Serializable]
    class LapUpdate
    {

    }

    [Serializable]
    class IntermittantUpdate
    {
        public float TC;
        public float Abs;
        public float BrakeBias;

        public int mfdTyreSet;
        public float mfdFuelToAdd;
        public float mfdTyrePressureLF;
        public float mfdTyrePressureRF;
        public float mfdTyrePressureLR;
        public float mfdTyrePressureRR;
        
        public int currentTyreSet;
        public int strategyTyreSet;
    }

    [Serializable]
    class OnDemandUpdate
    {
        public float TC;
        public float Abs;
        public float BrakeBias;
    }
}
