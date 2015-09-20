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
   /// Implements the MonitoringInfo profile message.
   /// </summary>
   public class MonitoringInfoMesg : Mesg
   {
      #region Fields
      #endregion

      #region Constructors
      public MonitoringInfoMesg() : base(Profile.mesgs[Profile.MonitoringInfoIndex])
      {
      }

      public MonitoringInfoMesg(Mesg mesg) : base(mesg)
      {
      }
      #endregion // Constructors

      #region Methods
      ///<summary>
      /// Retrieves the Timestamp field
      /// Units: s</summary>
      /// <returns>Returns DateTime representing the Timestamp field</returns>
      public DateTime GetTimestamp()
      {
         return TimestampToDateTime((uint?)GetFieldValue(253, 0, Fit.SubfieldIndexMainField));
      }

      

      

      /// <summary>
      /// Set Timestamp field
      /// Units: s</summary>
      /// <param name="timestamp_">Nullable field value to be set</param>
      public void SetTimestamp(DateTime timestamp_)
      {
         SetFieldValue(253, 0, timestamp_.GetTimeStamp(), Fit.SubfieldIndexMainField);
      }
      
      ///<summary>
      /// Retrieves the LocalTimestamp field
      /// Units: s
      /// Comment: Use to convert activity timestamps to local time if device does not support time zone and daylight savings time correction.</summary>
      /// <returns>Returns nullable uint representing the LocalTimestamp field</returns>
      public uint? GetLocalTimestamp()
      {
         return (uint?)GetFieldValue(0, 0, Fit.SubfieldIndexMainField);
      }

      

      

      /// <summary>
      /// Set LocalTimestamp field
      /// Units: s
      /// Comment: Use to convert activity timestamps to local time if device does not support time zone and daylight savings time correction.</summary>
      /// <param name="localTimestamp_">Nullable field value to be set</param>
      public void SetLocalTimestamp(uint? localTimestamp_)
      {
         SetFieldValue(0, 0, localTimestamp_, Fit.SubfieldIndexMainField);
      }
      
      
      /// <summary>
      ///
      /// </summary>
      /// <returns>returns number of elements in field ActivityType</returns>
      public int GetNumActivityType()
      {
         return GetNumFieldValues(1, Fit.SubfieldIndexMainField);
      }

      ///<summary>
      /// Retrieves the ActivityType field</summary>
      /// <param name="index">0 based index of ActivityType element to retrieve</param>
      /// <returns>Returns nullable ActivityType enum representing the ActivityType field</returns>
      public ActivityType? GetActivityType(int index)
      {
         object obj = GetFieldValue(1, index, Fit.SubfieldIndexMainField);
         ActivityType? value = obj == null ? (ActivityType?)null : (ActivityType)obj;
         return value;
      }

      

      

      /// <summary>
      /// Set ActivityType field</summary>
      /// <param name="index">0 based index of activity_type</param>
      /// <param name="activityType_">Nullable field value to be set</param>
      public void SetActivityType(int index, ActivityType? activityType_)
      {
         SetFieldValue(1, index, activityType_, Fit.SubfieldIndexMainField);
      }
      
      
      /// <summary>
      ///
      /// </summary>
      /// <returns>returns number of elements in field CyclesToDistance</returns>
      public int GetNumCyclesToDistance()
      {
         return GetNumFieldValues(3, Fit.SubfieldIndexMainField);
      }

      ///<summary>
      /// Retrieves the CyclesToDistance field
      /// Units: m/cycle
      /// Comment: Indexed by activity_type</summary>
      /// <param name="index">0 based index of CyclesToDistance element to retrieve</param>
      /// <returns>Returns nullable float representing the CyclesToDistance field</returns>
      public float? GetCyclesToDistance(int index)
      {
         return (float?)GetFieldValue(3, index, Fit.SubfieldIndexMainField);
      }

      

      

      /// <summary>
      /// Set CyclesToDistance field
      /// Units: m/cycle
      /// Comment: Indexed by activity_type</summary>
      /// <param name="index">0 based index of cycles_to_distance</param>
      /// <param name="cyclesToDistance_">Nullable field value to be set</param>
      public void SetCyclesToDistance(int index, float? cyclesToDistance_)
      {
         SetFieldValue(3, index, cyclesToDistance_, Fit.SubfieldIndexMainField);
      }
      
      
      /// <summary>
      ///
      /// </summary>
      /// <returns>returns number of elements in field CyclesToCalories</returns>
      public int GetNumCyclesToCalories()
      {
         return GetNumFieldValues(4, Fit.SubfieldIndexMainField);
      }

      ///<summary>
      /// Retrieves the CyclesToCalories field
      /// Units: kcal/cycle
      /// Comment: Indexed by activity_type</summary>
      /// <param name="index">0 based index of CyclesToCalories element to retrieve</param>
      /// <returns>Returns nullable float representing the CyclesToCalories field</returns>
      public float? GetCyclesToCalories(int index)
      {
         return (float?)GetFieldValue(4, index, Fit.SubfieldIndexMainField);
      }

      

      

      /// <summary>
      /// Set CyclesToCalories field
      /// Units: kcal/cycle
      /// Comment: Indexed by activity_type</summary>
      /// <param name="index">0 based index of cycles_to_calories</param>
      /// <param name="cyclesToCalories_">Nullable field value to be set</param>
      public void SetCyclesToCalories(int index, float? cyclesToCalories_)
      {
         SetFieldValue(4, index, cyclesToCalories_, Fit.SubfieldIndexMainField);
      }
      
      ///<summary>
      /// Retrieves the RestingMetabolicRate field
      /// Units: kcal / day</summary>
      /// <returns>Returns nullable ushort representing the RestingMetabolicRate field</returns>
      public ushort? GetRestingMetabolicRate()
      {
         return (ushort?)GetFieldValue(5, 0, Fit.SubfieldIndexMainField);
      }

      

      

      /// <summary>
      /// Set RestingMetabolicRate field
      /// Units: kcal / day</summary>
      /// <param name="restingMetabolicRate_">Nullable field value to be set</param>
      public void SetRestingMetabolicRate(ushort? restingMetabolicRate_)
      {
         SetFieldValue(5, 0, restingMetabolicRate_, Fit.SubfieldIndexMainField);
      }
      
      #endregion // Methods
   } // Class
} // namespace
