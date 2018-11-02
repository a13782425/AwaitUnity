using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Utils
{
    public static byte RedNormal(float x, float y)
    {
        //return (char)(_sq(cos(atan2(j - 512, i - 512) / 2)) * 255);

        return (byte)(Mathf.Pow(Mathf.Cos(Mathf.Atan2(y - 512f, x - 512f) / 2f), 2) * 255);

    }
    public static byte GreenNormal(float x, float y)
    {
        return (byte)(Mathf.Pow(Mathf.Cos(Mathf.Atan2(y - 512f, x - 512f) / 2f - 2 * Mathf.Acos(-1) / 3), 2) * 255);

    }
    public static byte BlueNormal(float x, float y)
    {
        return (byte)(Mathf.Pow(Mathf.Cos(Mathf.Atan2(y - 512f, x - 512f) / 2f + 2 * Mathf.Acos(-1) / 3), 2) * 255);
    }

    public static byte RedMandelbrot(float x, float y)
    {
        float w = 0, h = 0;
        int k;
        for (k = 0; k++ < 256;)
        {
            float a = w * w - h * h + (x - 768.0f) / 512f;
            h = 2f * w * h + (y - 512.0f) / 512f;
            w = a;
            if (w * w + h * h > 4)
                break;
        }
        return (byte)(Mathf.Log(k) * 47f);
    }
    public static byte GreenMandelbrot(float x, float y)
    {
        float w = 0, h = 0;
        int k;
        for (k = 0; k++ < 256;)
        {
            float a = w * w - h * h + (x - 768.0f) / 512f;
            h = 2f * w * h + (y - 512.0f) / 512f;
            w = a;
            if (w * w + h * h > 4f)
                break;
        }
        return (byte)(Mathf.Log(k) * 47f);
    }
    public static byte BlueMandelbrot(float x, float y)
    {
        float w = 0, h = 0; int k; for (k = 0; k++ < 256;) { float a = w * w - h * h + (x - 768.0f) / 512f; h = 2f * w * h + (y - 512.0f) / 512f; w = a; if (w * w + h * h > 4f) break; }
        return (byte)(128f - Mathf.Log(k) * 23f);
    }


    public static byte RedMandelbrot2(float x, float y)
    {
        double a = 0, b = 0, c, d, n = 0;

        while ((c = a * a) + (d = b * b) < 4 && n++ < 880)
        {
            b = 2 * a * b + y * 8e-9 - .645411;
            a = c - d + x * 8e-9 + .356888;
        }

        return (byte)(255 * Math.Pow((n - 80) / 800, 3.0));
    }
    public static byte GreenMandelbrot2(float x, float y)
    {
        double a = 0, b = 0, c, d, n = 0;

        while ((c = a * a) + (d = b * b) < 4 && n++ < 880)
        {
            b = 2 * a * b + y * 8e-9 - .645411;
            a = c - d + x * 8e-9 + .356888;
        }

        return (byte)(255 * Math.Pow((n - 80) / 800, 0.7));
    }
    public static byte BlueMandelbrot2(float x, float y)
    {
        double a = 0, b = 0, c, d, n = 0;

        while ((c = a * a) + (d = b * b) < 4 && n++ < 880)
        {
            b = 2 * a * b + y * 8e-9 - .645411;
            a = c - d + x * 8e-9 + .356888;
        }

        return (byte)(255 * Math.Pow((n - 80) / 800, .5));
    }

}

