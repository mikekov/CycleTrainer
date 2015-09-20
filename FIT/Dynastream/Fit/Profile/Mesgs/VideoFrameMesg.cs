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
   /// Implements the VideoFrame profile message.
   /// </summary>
   public class VideoFrameMesg : Mesg
   {
      #region Fields
      #endregion

      #region Constructors
      public VideoFrameMesg() : base(Profile.mesgs[Profile.VideoFrameIndex])
      {
      }

      public VideoFrameMesg(Mesg mesg) : base(mesg)
      {
      }
      #endregion // Constructors

      #region Methods
      ///<summary>
      /// Retrieves the Timestamp field
      /// Units: s
      /// Comment: Whole second part of the timestamp</summary>
      /// <returns>Returns DateTime representing the Timestamp field</returns>
      public DateTime GetTimestamp()
      {
         return TimestampToDateTime((uint?)GetFieldValue(253, 0, Fit.SubfieldIndexMainField));
      }

      

      

      /// <summary>
      /// Set Timestamp field
      /// Units: s
      /// Comment: Whole second part of the timestamp</summary>
      /// <param name="timestamp_">Nullable field value to be set</param>
      public void SetTimestamp(DateTime timestamp_)
      {
         SetFieldValue(253, 0, timestamp_.GetTimeStamp(), Fit.SubfieldIndexMainField);
      }
      
      ///<summary>
      /// Retrieves the TimestampMs field
      /// Units: ms
      /// Comment: Millisecond part of the timestamp.</summary>
      /// <returns>Returns nullable ushort representing the TimestampMs field</returns>
      public ushort? GetTimestampMs()
      {
         return (ushort?)GetFieldValue(0, 0, Fit.SubfieldIndexMainField);
      }

      

      

      /// <summary>
      /// Set TimestampMs field
      /// Units: ms
      /// Comment: Millisecond part of the timestamp.</summary>
      /// <param name="timestampMs_">Nullable field value to be set</param>
      public void SetTimestampMs(ushort? timestampMs_)
      {
         SetFieldValue(0, 0, timestampMs_, Fit.SubfieldIndexMainField);
      }
      
      ///<summary>
      /// Retrieves the FrameNumber field
      /// Comment: Number of the frame that the timestamp and timestamp_ms correlate to</summary>
      /// <returns>Returns nullable uint representing the FrameNumber field</returns>
      public uint? GetFrameNumber()
      {
         return (uint?)GetFieldValue(1, 0, Fit.SubfieldIndexMainField);
      }

      

      

      /// <summary>
      /// Set FrameNumber field
      /// Comment: Number of the frame that the timestamp and timestamp_ms correlate to</summary>
      /// <param name="frameNumber_">Nullable field value to be set</param>
      public void SetFrameNumber(uint? frameNumber_)
      {
         SetFieldValue(1, 0, frameNumber_, Fit.SubfieldIndexMainField);
      }
      
      #endregion // Methods
   } // Class
} // namespace
