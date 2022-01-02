﻿using System.Drawing;
using Iot.Device.Common;
using Iot.Device.SenseHat;
using Iot.Device.SenseHat.Extension;

using SenseHat sh = new SenseHat();

// LedMatrix displaying
foreach (var rotation in Enum.GetValues<Rotation>())
{
    Console.WriteLine("Showing letters - {0}", rotation);
    int index = 0;
    foreach (var letter in "@9876543210~!")
    {
        if (index % 2 == 0)
            sh.LedMatrix.ShowLetter(letter, Color.Blue, Color.Black, rotation);
        else
            sh.LedMatrix.ShowMessage(letter.ToString(), foreColor: Color.Blue, rotation: rotation);
        Thread.Sleep(200);
    }

    var message = " »»»»0123456789»»»»ABCDEFG»»»»ΔΘΠΣΦΨΩαβζ»»»» ";
    int speedInMs = 20;

    Console.WriteLine("Scrolling message to left- {0}", rotation);

    sh.LedMatrix.ShowMessage(message, speedInMs, Color.Blue, Color.Black, rotation, Direction.Left);

    Console.WriteLine("Scrolling messageto right- {0}", rotation);

    sh.LedMatrix.ShowMessage(message, speedInMs, Color.Red, Color.Black, rotation, Direction.Right);

    Console.WriteLine("Scrolling message to up- {0}", rotation);

    sh.LedMatrix.ShowMessage(message, speedInMs, Color.Green, Color.Black, rotation, Direction.Up);

    Console.WriteLine("Scrolling message to down- {0}", rotation);

    sh.LedMatrix.ShowMessage(message, speedInMs, Color.Yellow, Color.Black, rotation, Direction.Down);
}


int n = 0;
int x = 3, y = 3;
// set this to the current sea level pressure in the area for correct altitude readings
var defaultSeaLevelPressure = WeatherHelper.MeanSeaLevel;

while (true)
{
    Console.Clear();

    (int dx, int dy, bool holding) = JoystickState(sh);

    if (holding)
    {
        n++;
    }

    x = (x + 8 + dx) % 8;
    y = (y + 8 + dy) % 8;

    sh.Fill(n % 2 == 0 ? Color.DarkBlue : Color.DarkRed);
    sh.SetPixel(x, y, Color.Yellow);

    var tempValue = sh.Temperature;
    var temp2Value = sh.Temperature2;
    var preValue = sh.Pressure;
    var humValue = sh.Humidity;
    var accValue = sh.Acceleration;
    var angValue = sh.AngularRate;
    var magValue = sh.MagneticInduction;
    var altValue = WeatherHelper.CalculateAltitude(preValue, defaultSeaLevelPressure, tempValue);

    Console.WriteLine($"Temperature Sensor 1: {tempValue.DegreesCelsius:0.#}\u00B0C");
    Console.WriteLine($"Temperature Sensor 2: {temp2Value.DegreesCelsius:0.#}\u00B0C");
    Console.WriteLine($"Pressure: {preValue.Hectopascals:0.##} hPa");
    Console.WriteLine($"Altitude: {altValue.Meters:0.##} m");
    Console.WriteLine($"Acceleration: {sh.Acceleration} g");
    Console.WriteLine($"Angular rate: {sh.AngularRate} DPS");
    Console.WriteLine($"Magnetic induction: {sh.MagneticInduction} gauss");
    Console.WriteLine($"Relative humidity: {humValue.Percent:0.#}%");
    Console.WriteLine($"Heat index: {WeatherHelper.CalculateHeatIndex(tempValue, humValue).DegreesCelsius:0.#}\u00B0C");
    Console.WriteLine($"Dew point: {WeatherHelper.CalculateDewPoint(tempValue, humValue).DegreesCelsius:0.#}\u00B0C");

    Thread.Sleep(1000);
}

(int, int, bool) JoystickState(SenseHat sh)
{
    sh.ReadJoystickState();

    int dx = 0;
    int dy = 0;

    if (sh.HoldingUp)
    {
        dy--; // y goes down
    }

    if (sh.HoldingDown)
    {
        dy++;
    }

    if (sh.HoldingLeft)
    {
        dx--;
    }

    if (sh.HoldingRight)
    {
        dx++;
    }

    return (dx, dy, sh.HoldingButton);
}
