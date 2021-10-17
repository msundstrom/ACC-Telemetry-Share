using AssettoCorsaSharedMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ACCTelemetrySharing
{
    public interface ACCEvent
    {
        string eventName { get; }
    }

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
                CarModel = staticInfo.CarModel,
                iCurrentTime = graphics.iCurrentTime,
                iLastTime = graphics.iLastTime,
                iBestTime = graphics.iBestTime,
                sessionTimeLeft = graphics.sessionTimeLeft,
                iEstimatedLapTime = graphics.iEstimatedLapTime,
                isDeltaPositive = graphics.isDeltaPositive,
                isValidLap = graphics.isValidLap,
                DriverStintTotalTimeLeft = graphics.DriverStintTotalTimeLeft,
                DriverStintTimeLeft = graphics.DriverStintTimeLeft,
                Clock = graphics.Clock,
                GlobalYellow = graphics.GlobalYellow,
                GlobalYellow1 = graphics.GlobalYellow1,
                GlobalYellow2 = graphics.GlobalYellow2,
                GlobalYellow3 = graphics.GlobalYellow3,
                GlobalWhite = graphics.GlobalWhite,
                GlobalGreen = graphics.GlobalGreen,
                GlobalChequered = graphics.GlobalChequered,
                GlobalRed = graphics.GlobalRed,
                FlagType = graphics.flag,
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
                tyrePressure = physics.WheelsPressure,
                padLife = physics.padLife,
                discLife = physics.discLife,
                trackGripStatus = graphics.trackGripStatus,
                rainIntensity = graphics.rainIntensity,
                rainIntensityIn10min = graphics.rainIntensityIn10min,
                rainIntensityIn30min = graphics.rainIntensityIn30min,
                windSpeed = graphics.windSpeed,
                AirTemp = physics.AirTemp,
                RoadTemp = physics.RoadTemp,
                TC = graphics.TC,
                TCCut = graphics.TCCut,
                Abs = graphics.ABS,
                BrakeBias = physics.BrakeBias,
                EngineMap = graphics.EngineMap,
                IgnitionOn = physics.ignitionOn,
                lightsStage = graphics.lightsStage,
                rainLights = graphics.rainLights,
                directionLightsLeft = graphics.directionLightsLeft,
                directionLightsRight = graphics.directionLightsRight,
                engineMap = graphics.EngineMap,
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
                CarCoordinates = TransformCarCoordinates(graphics.CarCoordinates, staticInfo.NumCars)
            };
        }

        public static NewLapUpdate createNewLapUpdate(
            string shortName,
            LapUpdate lapUpdate,
            StintUpdate stintUpdate
            )
        {
            return new NewLapUpdate()
            {
                shortName = shortName,
                averageTyreTemp = lapUpdate.averageTemps().rawData(),
                averageTyrePsi = lapUpdate.averagePressures().rawData(),
                stintAverageLapTimeMs = stintUpdate.averageLapTimeMs(),
                predictedPadWear = stintUpdate.predictedPadLife().rawData(),
            };
        }

        public static PitInUpdate createPitInUpdate(
            string shortName,
            Graphics graphics
            )
        {
            return new PitInUpdate()
            {
                shortName = shortName,
                sessionTimeLeft = graphics.sessionTimeLeft,
            };
        }

        public static PitOutUpdate createPitOutUpdate(
            string shortName,
            Graphics graphics
            )
        {
            return new PitOutUpdate()
            {
                shortName = shortName,
                sessionTimeLeft = graphics.sessionTimeLeft,
            };
        }

        public static RoomCreateUpdate createRoomCreateUpdate(
            string shortName,
            string roomName
            ) {
            return new RoomCreateUpdate() {
                shortName = shortName,
                roomName = roomName,
            };
        }

        public static RoomConnectUpdate createRoomConnectUpdate(
            string shortName,
            string roomName
            ) {
            return new RoomConnectUpdate() {
                shortName = shortName,
                roomName = roomName,
            };
        }

        public static float[,] TransformCarCoordinates(float[] flatCoordinates, int numberOfCars)
        {

            var coordinates = new float[numberOfCars, 3];

            for (var i = 0; i < numberOfCars; i += 3)
            {
                for (var j = 0; j < 3; j++)
                {
                    coordinates[i, j] = flatCoordinates[i + j];
                }
            }

            return coordinates;
        }
    }

    [Serializable]
    class RealTimeUpdate : ACCEvent
    {
        public string eventName =>
            "REAL_TIME_UPDATE";
        // origin
        public string shortName;
        public string CarModel;

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
        public float Clock;

        // flags
        public int GlobalYellow;
        public int GlobalYellow1;
        public int GlobalYellow2;
        public int GlobalYellow3;
        public int GlobalWhite;
        public int GlobalGreen;
        public int GlobalChequered;
        public int GlobalRed;
        public AC_FLAG_TYPE FlagType;

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
        public float[] tyrePressure;

        // weather & track
        public ACC_TRACK_GRIP_STATUS trackGripStatus;
        public ACC_RAIN_INTENSITY rainIntensity;
        public ACC_RAIN_INTENSITY rainIntensityIn10min;
        public ACC_RAIN_INTENSITY rainIntensityIn30min;
        public float windSpeed;
        public float AirTemp;
        public float RoadTemp;

        // electronics
        public int TC;
        public int TCCut;
        public int Abs;
        public float BrakeBias;
        public int EngineMap;
        public int IgnitionOn;
        public int lightsStage;
        public int rainLights;
        public int directionLightsLeft;
        public int directionLightsRight;
        public int engineMap;

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
        public float[,] CarCoordinates;
    }

    [Serializable]
    class NewLapUpdate : ACCEvent
    {
        public string eventName =>
            "NEW_LAP_UPDATE";
        public string shortName;
        public float[] averageTyreTemp;
        public float[] averageTyrePsi;
        public int stintAverageLapTimeMs;
        public float[] predictedPadWear;

    }

    [Serializable]
    class PitInUpdate: ACCEvent
    {
        public string eventName =>
            "PIT_IN_UPDATE";
        public string shortName;
        public float sessionTimeLeft;
    }

    [Serializable]
    class PitOutUpdate: ACCEvent
    {
        public string eventName =>
            "PIT_OUT_UPDATE";
        public string shortName;
        public float sessionTimeLeft;
    }

    [Serializable]
    class RoomCreateUpdate : ACCEvent {
        public string eventName =>
            "ROOM_CREATE";
        public string shortName;
        public string roomName;
    }

    [Serializable]
    class RoomConnectUpdate: ACCEvent {
        public string eventName =>
            "ROOM_CONNECT";
        public string shortName;
        public string roomName;
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
