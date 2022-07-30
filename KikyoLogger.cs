using System;
using MelonLoader;

namespace KikyoMod;

public static class KikyoLogger
{
    private static readonly MelonLogger.Instance Instance = new("KikyoMod", ConsoleColor.White);

    public static void Msg(object obj)
    {
        Instance.Msg(obj);
    }

    public static void Msg(string txt)
    {
        Instance.Msg(txt);
    }

    public static void Msg(string txt, params object[] args)
    {
        Instance.Msg(txt, args);
    }

    public static void Msg(ConsoleColor txtColor, string txt)
    {
        Instance.Msg(txtColor, txt);
    }

    public static void Msg(ConsoleColor txtColor, string txt, params object[] args)
    {
        Instance.Msg(txtColor, txt, args);
    }

    public static void Warning(object obj)
    {
        Instance.Warning(obj);
    }

    public static void Warning(string txt)
    {
        Instance.Warning(txt);
    }

    public static void Warning(string txt, params object[] args)
    {
        Instance.Warning(txt, args);
    }

    public static void Error(object obj)
    {
        Instance.Error(obj);
    }

    public static void Error(string txt)
    {
        Instance.Error(txt);
    }

    public static void Error(string txt, params object[] args)
    {
        Instance.Error(txt, args);
    }

    public static void Error(string txt, Exception ex)
    {
        Instance.Error(txt, ex);
    }
}