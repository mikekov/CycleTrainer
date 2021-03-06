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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;


namespace Dynastream.Fit
{
   /// <summary>
   /// Implements the NmeaSentence profile message.
   /// </summary>
   public class NmeaSentenceMesg : Mesg
   {
      #region Fields
      #endregion

      #region Constructors
      public NmeaSentenceMesg() : base(Profile.mesgs[Profile.NmeaSentenceIndex])
      {
      }

      public NmeaSentenceMesg(Mesg mesg) : base(mesg)
      {
      }
      #endregion // Constructors

      #region Methods
      ///<summary>
      /// Retrieves the Timestamp field
      /// Units: s
      /// Comment: Timestamp message was output</summary>
      /// <returns>Returns DateTime representing the Timestamp field</returns>
      public DateTime GetTimestamp()
      {
         return TimestampToDateTime((uint?)GetFieldValue(253, 0, Fit.SubfieldIndexMainField));
      }

      

      

      /// <summary>
      /// Set Timestamp field
      /// Units: s
      /// Comment: Timestamp message was output</summary>
      /// <param name="timestamp_">Nullable field value to be set</param>
      public void SetTimestamp(DateTime timestamp_)
      {
         SetFieldValue(253, 0, timestamp_.GetTimeStamp(), Fit.SubfieldIndexMainField);
      }
      
      ///<summary>
      /// Retrieves the TimestampMs field
      /// Units: ms
      /// Comment: Fractional part of timestamp, added to timestamp</summary>
      /// <returns>Returns nullable ushort representing the TimestampMs field</returns>
      public ushort? GetTimestampMs()
      {
         return (ushort?)GetFieldValue(0, 0, Fit.SubfieldIndexMainField);
      }

      

      

      /// <summary>
      /// Set TimestampMs field
      /// Units: ms
      /// Comment: Fractional part of timestamp, added to timestamp</summary>
      /// <param name="timestampMs_">Nullable field value to be set</param>
      public void SetTimestampMs(ushort? timestampMs_)
      {
         SetFieldValue(0, 0, timestampMs_, Fit.SubfieldIndexMainField);
      }
      
      ///<summary>
      /// Retrieves the Sentence field
      /// Comment: NMEA sentence</summary>
      /// <returns>Returns byte[] representing the Sentence field</returns>
      public byte[] GetSentence()
      {
         return (byte[])GetFieldValue(1, 0, Fit.SubfieldIndexMainField);
      }

      
      ///<summary>
      /// Retrieves the Sentence field
      /// Comment: NMEA sentence</summary>
      /// <returns>Returns String representing the Sentence field</returns>
      public String GetSentenceAsString()
      {
         return Encoding.UTF8.GetString((byte[])GetFieldValue(1, 0, Fit.SubfieldIndexMainField));
      }
      

      
      ///<summary>
      /// Set Sentence field
      /// Comment: NMEA sentence</summary>
      /// <returns>Returns String representing the Sentence field</returns>
      public void SetSentence(String sentence_)
      {
         SetFieldValue(1, 0, System.Text.Encoding.UTF8.GetBytes(sentence_), Fit.SubfieldIndexMainField);
      }
      

      /// <summary>
      /// Set Sentence field
      /// Comment: NMEA sentence</summary>
      /// <param name="sentence_">field value to be set</param>
      public void SetSentence(byte[] sentence_)
      {
         SetFieldValue(1, 0, sentence_, Fit.SubfieldIndexMainField);
      }
      
      #endregion // Methods
   } // Class
} // namespace
