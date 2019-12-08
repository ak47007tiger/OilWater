using System;
using System.Text;
using xxHashSharp;

public static class GuiStringExtension
{
    public static uint GuiGetHashCode(this string str)
    {
        byte[] input = Encoding.UTF8.GetBytes(str);
        return xxHash.CalculateHash(input);
    }
}

