namespace Timbratura_Testo.Services
{
    public class TimeRounding
    {
        public DateTime RoundTime(DateTime time, int interval = 15)
        {
            int minutesInterval = 15;
            int totalMinutes = ((time.Hour * 60) + time.Minute + (minutesInterval / 2)) / minutesInterval * minutesInterval;
            int roundedHours = totalMinutes / 60;
            int roundedMinutes = totalMinutes % 60;

            return new DateTime(time.Year, time.Month, time.Day, roundedHours, roundedMinutes, 0);
        }
    }
    
}
