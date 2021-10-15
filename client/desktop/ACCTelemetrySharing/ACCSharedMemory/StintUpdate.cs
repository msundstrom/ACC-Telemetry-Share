using AssettoCorsaSharedMemory;
using System;
using System.Collections.Generic;
using System.Linq;

public static class Extensions
{   public static TimeSpan Average(this IEnumerable<TimeSpan> timeSpans)
    {
        IEnumerable<double> ticksPerTimeSpan = timeSpans.Select(t => t.TotalMilliseconds);
        double averageMs = ticksPerTimeSpan.Average();

        TimeSpan averageTimeSpan = TimeSpan.FromMilliseconds(averageMs);

        return averageTimeSpan;
    }
}


namespace ACCTelemetrySharing
{
    public class StintUpdate
    {
        public List<TimeSpan> lapTimes = new List<TimeSpan>();
        public TimeSpan stintStartTime;
        public TimeSpan stintEndTime = new TimeSpan();
        public List<LapUpdate.Wheels> brakePadWear = new List<LapUpdate.Wheels>();
        public List<LapUpdate.Wheels> brakeDiscWear = new List<LapUpdate.Wheels>();
        public int stintOutlap;

        private double startingPadValue = 29.0;
        private double padCriticalValue = 10.0;
        private double startinDiscValue = 32.0;

        //public StintUpdate(GameData data)
        //{
        //    lapTimes = new List<TimeSpan>();
        //    stintStartTime = data.NewData.SessionTimeLeft;
        //    stintEndTime = new TimeSpan();
        //    stintOutlap = data.NewData.CurrentLap;
        //}

        public StintUpdate()
        {
            lapTimes = new List<TimeSpan>();
            stintStartTime = new TimeSpan();
            stintEndTime = new TimeSpan();
            stintOutlap = 1;
        }

        private TimeSpan? averageLaptime()
        {
            if (lapTimes.Count <= 1)
            {
                return null;
            }

            //exclude outlap
            List<TimeSpan> stintLaps = lapTimes.GetRange(1, lapTimes.Count - 1);
            TimeSpan averageTime = Extensions.Average(stintLaps);

            return averageTime;
        }

        public String formattedAverageLapTime()
        {
            if (averageLaptime() == null)
            {
                return "-";
            }

            TimeSpan averageTime = (TimeSpan)averageLaptime();
            return  averageTime.Minutes.ToString("D2") + 
                    ":" + averageTime.Seconds.ToString("D2") + 
                    "." + averageTime.Milliseconds.ToString("D3");
        }

        public int averageLapTimeMs()
        {
            if (averageLaptime() == null)
            {
                return -1;
            }

            TimeSpan averageTime = (TimeSpan)averageLaptime();

            return (int)Math.Floor(averageTime.TotalMilliseconds);
        }

        public LapUpdate.Wheels averageBrakeDiscWear()
        {
            if (brakeDiscWear.Count() == 0) {
                return new LapUpdate.Wheels(-1);
            }

            LapUpdate.Wheels totalWear = new LapUpdate.Wheels();

            int snapshotCount = brakeDiscWear.Count() - 1;

            for (int i = 0; i < snapshotCount; i++)
            {
                totalWear.FL += brakeDiscWear[i].FL - brakeDiscWear[i + 1].FL;
                totalWear.FR += brakeDiscWear[i].FR - brakeDiscWear[i + 1].FR;
                totalWear.RL += brakeDiscWear[i].RL - brakeDiscWear[i + 1].RL;
                totalWear.RR += brakeDiscWear[i].RR - brakeDiscWear[i + 1].RR;
            }

            return new LapUpdate.Wheels(
                totalWear.FL / snapshotCount,
                totalWear.FR / snapshotCount,
                totalWear.RL / snapshotCount,
                totalWear.RR / snapshotCount
                );
        }

        public LapUpdate.Wheels predictedPadLife()
        {
            LapUpdate.Wheels averagePadWear = averageBrakePadWear();
            if (averagePadWear.FL == -1) {
                return averagePadWear;
            }

            double padPredicatedLifeFL = (brakePadWear.Last().FL - padCriticalValue) / averagePadWear.FL * averageLapTimeMs();
            double padPredicatedLifeFR = (brakePadWear.Last().FR - padCriticalValue) / averagePadWear.FR * averageLapTimeMs();
            double padPredicatedLifeRL = (brakePadWear.Last().RL - padCriticalValue) / averagePadWear.RL * averageLapTimeMs();
            double padPredicatedLifeRR = (brakePadWear.Last().RR - padCriticalValue) / averagePadWear.RR * averageLapTimeMs();

            return new LapUpdate.Wheels(
                padPredicatedLifeFL,
                padPredicatedLifeFR,
                padPredicatedLifeRL,
                padPredicatedLifeRR
                );
        }

        public LapUpdate.Wheels averageBrakePadWear()
        {
            if (brakePadWear.Count() == 0) {
                return new LapUpdate.Wheels(-1);
            }

            LapUpdate.Wheels totalWear = new LapUpdate.Wheels();

            int snapshotCount = brakePadWear.Count() - 1;

            for (int i = 0; i < snapshotCount; i++)
            {
                totalWear.FL += brakePadWear[i].FL - brakePadWear[i + 1].FL;
                totalWear.FR += brakePadWear[i].FR - brakePadWear[i + 1].FR;
                totalWear.RL += brakePadWear[i].RL - brakePadWear[i + 1].RL;
                totalWear.RR += brakePadWear[i].RR - brakePadWear[i + 1].RR;
            }

            return new LapUpdate.Wheels(
                totalWear.FL / snapshotCount,
                totalWear.FR / snapshotCount,
                totalWear.RL / snapshotCount,
                totalWear.RR / snapshotCount
                );
        }

        public void update(Graphics graphics, Physics physics, StaticInfo staticInfo)
        {
            brakeDiscWear.Add(new LapUpdate.Wheels(physics.discLife));
            brakePadWear.Add(new LapUpdate.Wheels(physics.padLife));
            lapTimes.Add(new TimeSpan(0, 0, 0, 0, graphics.iLastTime));
        }

        public void updatePhysicsProps(Physics graphics)
        {
            brakeDiscWear.Add(new LapUpdate.Wheels(
                graphics.discLife[0],
                graphics.discLife[1],
                graphics.discLife[2],
                graphics.discLife[3]
            ));

            if (brakeDiscWear.Count > 10)
            {
                brakeDiscWear.RemoveAt(0);
            }

            brakePadWear.Add(new LapUpdate.Wheels(
                graphics.padLife[0],
                graphics.padLife[1],
                graphics.padLife[2],
                graphics.padLife[3]
            ));

            if (brakePadWear.Count > 10)
            {
                brakePadWear.RemoveAt(0);
            }
        }
    }
}
