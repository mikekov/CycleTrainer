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
   /// Implements the ThreeDSensorCalibration profile message.
   /// </summary>
   public class ThreeDSensorCalibrationMesg : Mesg
   {
      #region Fields
      static class CalibrationFactorSubfield
      {
         public static ushort AccelCalFactor = 0;
         public static ushort GyroCalFactor = 1;
         public static ushort Subfields = 2;
         public static ushort Active = Fit.SubfieldIndexActiveSubfield;
         public static ushort MainField = Fit.SubfieldIndexMainField;
      }
      #endregion

      #region Constructors
      public ThreeDSensorCalibrationMesg() : base(Profile.mesgs[Profile.ThreeDSensorCalibrationIndex])
      {
      }

      public ThreeDSensorCalibrationMesg(Mesg mesg) : base(mesg)
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
      /// Retrieves the SensorType field
      /// Comment: Indicates which sensor the calibration is for</summary>
      /// <returns>Returns nullable SensorType enum representing the SensorType field</returns>
      public SensorType? GetSensorType()
      {
         object obj = GetFieldValue(0, 0, Fit.SubfieldIndexMainField);
         SensorType? value = obj == null ? (SensorType?)null : (SensorType)obj;
         return value;
      }

      

      

      /// <summary>
      /// Set SensorType field
      /// Comment: Indicates which sensor the calibration is for</summary>
      /// <param name="sensorType_">Nullable field value to be set</param>
      public void SetSensorType(SensorType? sensorType_)
      {
         SetFieldValue(0, 0, sensorType_, Fit.SubfieldIndexMainField);
      }
      
      ///<summary>
      /// Retrieves the CalibrationFactor field
      /// Comment: Calibration factor used to convert from raw ADC value to degrees, g,  etc.</summary>
      /// <returns>Returns nullable uint representing the CalibrationFactor field</returns>
      public uint? GetCalibrationFactor()
      {
         return (uint?)GetFieldValue(1, 0, Fit.SubfieldIndexMainField);
      }

      

      

      /// <summary>
      /// Set CalibrationFactor field
      /// Comment: Calibration factor used to convert from raw ADC value to degrees, g,  etc.</summary>
      /// <param name="calibrationFactor_">Nullable field value to be set</param>
      public void SetCalibrationFactor(uint? calibrationFactor_)
      {
         SetFieldValue(1, 0, calibrationFactor_, Fit.SubfieldIndexMainField);
      }
      

      /// <summary>
      /// Retrieves the AccelCalFactor subfield
      /// Units: g
      /// Comment: Accelerometer calibration factor</summary>
      /// <returns>Nullable uint representing the AccelCalFactor subfield</returns>
      public uint? GetAccelCalFactor()
      {
         return (uint?)GetFieldValue(1, 0, CalibrationFactorSubfield.AccelCalFactor);
      }

      /// <summary>
      ///
      /// Set AccelCalFactor subfield
      /// Units: g
      /// Comment: Accelerometer calibration factor</summary>
      /// <param name="accelCalFactor">Subfield value to be set</param>
      public void SetAccelCalFactor(uint? accelCalFactor)
      {
         SetFieldValue(1, 0, accelCalFactor, CalibrationFactorSubfield.AccelCalFactor);
      }

      /// <summary>
      /// Retrieves the GyroCalFactor subfield
      /// Units: deg/s
      /// Comment: Gyro calibration factor</summary>
      /// <returns>Nullable uint representing the GyroCalFactor subfield</returns>
      public uint? GetGyroCalFactor()
      {
         return (uint?)GetFieldValue(1, 0, CalibrationFactorSubfield.GyroCalFactor);
      }

      /// <summary>
      ///
      /// Set GyroCalFactor subfield
      /// Units: deg/s
      /// Comment: Gyro calibration factor</summary>
      /// <param name="gyroCalFactor">Subfield value to be set</param>
      public void SetGyroCalFactor(uint? gyroCalFactor)
      {
         SetFieldValue(1, 0, gyroCalFactor, CalibrationFactorSubfield.GyroCalFactor);
      }
      ///<summary>
      /// Retrieves the CalibrationDivisor field
      /// Units: counts
      /// Comment: Calibration factor divisor</summary>
      /// <returns>Returns nullable uint representing the CalibrationDivisor field</returns>
      public uint? GetCalibrationDivisor()
      {
         return (uint?)GetFieldValue(2, 0, Fit.SubfieldIndexMainField);
      }

      

      

      /// <summary>
      /// Set CalibrationDivisor field
      /// Units: counts
      /// Comment: Calibration factor divisor</summary>
      /// <param name="calibrationDivisor_">Nullable field value to be set</param>
      public void SetCalibrationDivisor(uint? calibrationDivisor_)
      {
         SetFieldValue(2, 0, calibrationDivisor_, Fit.SubfieldIndexMainField);
      }
      
      ///<summary>
      /// Retrieves the LevelShift field
      /// Comment: Level shift value used to shift the ADC value back into range</summary>
      /// <returns>Returns nullable uint representing the LevelShift field</returns>
      public uint? GetLevelShift()
      {
         return (uint?)GetFieldValue(3, 0, Fit.SubfieldIndexMainField);
      }

      

      

      /// <summary>
      /// Set LevelShift field
      /// Comment: Level shift value used to shift the ADC value back into range</summary>
      /// <param name="levelShift_">Nullable field value to be set</param>
      public void SetLevelShift(uint? levelShift_)
      {
         SetFieldValue(3, 0, levelShift_, Fit.SubfieldIndexMainField);
      }
      
      
      /// <summary>
      ///
      /// </summary>
      /// <returns>returns number of elements in field OffsetCal</returns>
      public int GetNumOffsetCal()
      {
         return GetNumFieldValues(4, Fit.SubfieldIndexMainField);
      }

      ///<summary>
      /// Retrieves the OffsetCal field
      /// Comment: Internal calibration factors, one for each: xy, yx, zx</summary>
      /// <param name="index">0 based index of OffsetCal element to retrieve</param>
      /// <returns>Returns nullable int representing the OffsetCal field</returns>
      public int? GetOffsetCal(int index)
      {
         return (int?)GetFieldValue(4, index, Fit.SubfieldIndexMainField);
      }

      

      

      /// <summary>
      /// Set OffsetCal field
      /// Comment: Internal calibration factors, one for each: xy, yx, zx</summary>
      /// <param name="index">0 based index of offset_cal</param>
      /// <param name="offsetCal_">Nullable field value to be set</param>
      public void SetOffsetCal(int index, int? offsetCal_)
      {
         SetFieldValue(4, index, offsetCal_, Fit.SubfieldIndexMainField);
      }
      
      
      /// <summary>
      ///
      /// </summary>
      /// <returns>returns number of elements in field OrientationMatrix</returns>
      public int GetNumOrientationMatrix()
      {
         return GetNumFieldValues(5, Fit.SubfieldIndexMainField);
      }

      ///<summary>
      /// Retrieves the OrientationMatrix field
      /// Comment: 3 x 3 rotation matrix (row major)</summary>
      /// <param name="index">0 based index of OrientationMatrix element to retrieve</param>
      /// <returns>Returns nullable float representing the OrientationMatrix field</returns>
      public float? GetOrientationMatrix(int index)
      {
         return (float?)GetFieldValue(5, index, Fit.SubfieldIndexMainField);
      }

      

      

      /// <summary>
      /// Set OrientationMatrix field
      /// Comment: 3 x 3 rotation matrix (row major)</summary>
      /// <param name="index">0 based index of orientation_matrix</param>
      /// <param name="orientationMatrix_">Nullable field value to be set</param>
      public void SetOrientationMatrix(int index, float? orientationMatrix_)
      {
         SetFieldValue(5, index, orientationMatrix_, Fit.SubfieldIndexMainField);
      }
      
      #endregion // Methods
   } // Class
} // namespace