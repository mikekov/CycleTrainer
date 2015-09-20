#region Copyright
////////////////////////////////////////////////////////////////////////////////
// The following FIT Protocol software provided may be used with FIT protocol
// devices only and remains the copyrighted property of Dynastream Innovations Inc.
// The software is being provided on an "as-is" basis and as an accommodation,
// and therefore all warranties, representations, or guarantees of any kind
// (whether express, implied or statutory) including, without limitation,
// warranties of merchantability, non-infringement, or fitness for a particular
// purpose, are specifically disclaimed.
//
// Copyright 2015 Dynastream Innovations Inc.
////////////////////////////////////////////////////////////////////////////////
// ****WARNING****  This file is auto-generated!  Do NOT edit this file.
// Profile Version = 16.10Release
// Tag = development-akw-16.10.00-0
////////////////////////////////////////////////////////////////////////////////

#endregion

namespace Dynastream.Fit
{
   /// <summary>
   /// Implements the profile SportBits0 type as a class
   /// </summary>  
   public static class SportBits0 
   {
      public const byte Generic = 0x01;
      public const byte Running = 0x02;
      public const byte Cycling = 0x04;
      public const byte Transition = 0x08; // Mulitsport transition
      public const byte FitnessEquipment = 0x10;
      public const byte Swimming = 0x20;
      public const byte Basketball = 0x40;
      public const byte Soccer = 0x80;
      public const byte Invalid = (byte)0x00;   
      
   }
}

