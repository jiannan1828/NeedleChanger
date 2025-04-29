/*
 * ReceiveSurface.cs
 * 
 * Gocator 2000/2300 C# Sample
 * Copyright (C) 2013-2023 by LMI Technologies Inc.
 * 
 * Licensed under The MIT License.
 * Redistributions of files must retain the above copyright notice.
 *
 * Purpose: Connect to Gocator system and receive Surface data and translate to engineering units. Gocator must be in Surface Mode.
 * Ethernet output for the surface and/or intensity data must be enabled.
 */

using System;
using System.Runtime.InteropServices;
using Lmi3d.GoSdk;
using Lmi3d.Zen;
using Lmi3d.Zen.Io;
using Lmi3d.GoSdk.Messages;
//using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Drawing.Imaging;
using System.Data;
using System.Runtime.Remoting.Contexts;
using HalconDotNet;

namespace ReceiveSurface {
    static class Constants {
        public const string SENSOR_IP = "192.168.1.10"; // IP of the sensor used for sensor connection GoSystem_FindSensorByIpAddress() call.

        public const uint WEB_PORT = 8081;  // Using non-default web port number
        public const uint RECEIVE_DATA_TIMEOUT_USEC = 30000000; // 30 sec
    }

    public class DataContext {
        public double xResolution;
        public double yResolution;
        public double zResolution;
        public double xOffset;
        public double yOffset;
        public double zOffset;
        public uint serialNumber;
    }

    public struct GoPoints {
        public Int16 x;
        public Int16 y;
        public Int16 z;
    }

    public struct SurfacePoint {
        public double x;
        public double y;
        public double z;
        byte intensity;
    }

    class ReceiveSurface {
        public GoSystem      system;
        public GoAccelerator accelerator;
        public GoSensor      sensor;

        public void Init() {
            try {
                KApiLib.Construct();
                GoSdkLib.Construct();
                system = new GoSystem();
                accelerator = new GoAccelerator();
                KIpAddress ipAddress = KIpAddress.Parse(Constants.SENSOR_IP);
                GoDataSet dataSet = new GoDataSet();

                accelerator.WebPort = Constants.WEB_PORT;
                accelerator.Start();
                //if (system.SensorCount > 0)
                //    sensor = system.GetSensor(0);
                sensor = system.FindSensorByIpAddress(ipAddress);

                accelerator.Attach(sensor);

                sensor.Connect();
                system.EnableData(true);
            } catch (KException ex) {
                Console.WriteLine("Error: {0}", ex.ToString());
            }
        }

        public void SoftwareTrigger() {
            try {
                sensor.Trigger();
            } catch (KException ex) {
                Console.WriteLine("Error: {0}", ex.ToString());
            }
        }

        public void Start() {
            try {
                system.Start();
            } catch (KException ex) {
                Console.WriteLine("Error: {0}", ex.ToString());
            }
        }

        public void ReceiveData() {
            try {
                GoDataSet dataSet = new GoDataSet();
                bool isDataGet = false;

                while (true) {
                    if (isDataGet) { 
                        break;
                    }

                    dataSet = system.ReceiveData(30000000);

                    for (UInt32 i = 0; i < dataSet.Count; i++) {
                        GoDataMsg dataObj = (GoDataMsg)dataSet.Get(i);
                        switch (dataObj.MessageType) {
                            case GoDataMessageType.Stamp: {
                                GoStampMsg stampMsg = (GoStampMsg)dataObj;
                                for (UInt32 j = 0; j < stampMsg.Count; j++) {
                                    GoStamp stamp = stampMsg.Get(j);
                                    Console.WriteLine("Frame Index = {0}", stamp.FrameIndex);
                                    Console.WriteLine("Time Stamp = {0}", stamp.Timestamp);
                                    Console.WriteLine("Encoder Value = {0}", stamp.Encoder);
                                }
                            } break;

                            case GoDataMessageType.UniformSurface: {
                                GoUniformSurfaceMsg surfaceMsg = (GoUniformSurfaceMsg)dataObj;
                                long width = surfaceMsg.Width;
                                long length = surfaceMsg.Length;
                                long bufferSize = width * length;
                                IntPtr bufferPointer = surfaceMsg.Data;

                                Console.WriteLine("Uniform Surface received:");
                                Console.WriteLine(" Buffer width: {0}", width);
                                Console.WriteLine(" Buffer length: {0}", length);

                                if(width > 10) {
                                    //short[] ranges = new short[bufferSize];
                                    //Marshal.Copy(bufferPointer, ranges,0, ranges.Length);
                                    HObject hImage;
                                    HOperatorSet.GenImage1(out hImage, "int2", width, length, bufferPointer);
                                    HOperatorSet.WriteImage(hImage, "tiff", 0, "test.tif");

                                    isDataGet = true;
                                }
                            } break;
                        }  // end of switch (dataObj.MessageType) {
                    }  // end of for (UInt32 i = 0; i < dataSet.Count; i++) {
                }  // end of while (true) {
            } catch (KException ex) {
                Console.WriteLine("Error: {0}", ex.ToString());
            }
        }  // end of public void ReceiveData() {

        public void Stop() {
            try {
                system.Stop();
            } catch (KException ex) {
                Console.WriteLine("Error: {0}", ex.ToString());
            }
        }

        public void DeInit() {
            try {
                //accelerator.Detach(sensor);
                //accelerator.Stop();

                //accelerator.Dispose();
            } catch (KException ex) {
                Console.WriteLine("Error: {0}", ex.ToString());
            }   
        }

        static int MainTest(string[] args) {
            try {
                KApiLib.Construct();
                GoSdkLib.Construct();
                GoSystem system = new GoSystem();

                GoAccelerator accelerator = new GoAccelerator(); //////

                GoSensor sensor;
                KIpAddress ipAddress = KIpAddress.Parse(Constants.SENSOR_IP);
                GoDataSet dataSet = new GoDataSet();

                accelerator.WebPort = Constants.WEB_PORT;
                accelerator.Start();
                //if (system.SensorCount > 0)
                //    sensor = system.GetSensor(0);
                sensor = system.FindSensorByIpAddress(ipAddress);

                accelerator.Attach(sensor);

                sensor.Connect();

                GoSetup setup = sensor.Setup;
                setup.ScanMode = GoMode.Surface;
                system.EnableData(true);
                system.Start();
                Console.WriteLine("Waiting for Surface Data...");
                dataSet = system.ReceiveData(30000000);
                DataContext context = new DataContext();
                for (UInt32 i = 0; i < dataSet.Count; i++) {
                    GoDataMsg dataObj = (GoDataMsg)dataSet.Get(i);
                    switch (dataObj.MessageType) {
                        case GoDataMessageType.Stamp: {
                                GoStampMsg stampMsg = (GoStampMsg)dataObj;
                                for (UInt32 j = 0; j < stampMsg.Count; j++)
                                {
                                    GoStamp stamp = stampMsg.Get(j);
                                    Console.WriteLine("Frame Index = {0}", stamp.FrameIndex);
                                    Console.WriteLine("Time Stamp = {0}", stamp.Timestamp);
                                    Console.WriteLine("Encoder Value = {0}", stamp.Encoder);
                                }
                        } break;

                        case GoDataMessageType.UniformSurface: {
                                GoUniformSurfaceMsg surfaceMsg = (GoUniformSurfaceMsg)dataObj;
                                long width = surfaceMsg.Width;
                                long length = surfaceMsg.Length;
                                long bufferSize = width * length;
                                IntPtr bufferPointer = surfaceMsg.Data;

                                Console.WriteLine("Uniform Surface received:");
                                Console.WriteLine(" Buffer width: {0}", width);
                                Console.WriteLine(" Buffer length: {0}", length);

                                context.xResolution = (double)surfaceMsg.XResolution / 1000000;
                                context.yResolution = (double)surfaceMsg.YResolution / 1000000;
                                context.zResolution = (double)surfaceMsg.ZResolution / 1000000;
                                context.xOffset = (double)surfaceMsg.XOffset / 1000;
                                context.yOffset = (double)surfaceMsg.YOffset / 1000;
                                context.zOffset = (double)surfaceMsg.ZOffset / 1000;

                                short[] ranges = new short[bufferSize];

                                Marshal.Copy(bufferPointer, ranges, 0, ranges.Length);
                                //Marshal.Copy(bufferPointer, ranges, 0, ranges.Length);
                                ushort[] rangesUnsigned = new ushort[bufferSize];

                                for (int idx = 0; idx < bufferSize; idx++) {
                                    rangesUnsigned[idx] = (ushort)((int)ranges[idx] - short.MinValue);
                                }

                                byte[] rangesByte= new byte[bufferSize];

                                for (int idx = 0; idx < bufferSize; idx++) {
                                    rangesByte[idx] =(byte)((int)ranges[idx] - short.MinValue);
                                }

                                GCHandle pinnedArray = GCHandle.Alloc(rangesByte, GCHandleType.Pinned);
                                IntPtr pointer = pinnedArray.AddrOfPinnedObject();
                                // Do your stuff...

                                // Assuming bufferPointer points to valid image data in 16bpp format
                                if (width > 2) {
                                    try {
                                        // Create the Bitmap from the bufferPointer
                                        Bitmap bmp = new Bitmap ( (int)width,
                                                                  (int)length,
                                                                  (int)(width), // This is the stride (width * bytes per pixel)
                                                                  System.Drawing.Imaging.PixelFormat.Format8bppIndexed,
                                                                  pointer );

                                        ColorPalette palette = bmp.Palette;
                                        for (int idxx = 0; idxx < 256; idxx++) {
                                            // 設置每個顏色，這裡的顏色可以根據您的需求進行修改
                                            palette.Entries[idxx] = Color.FromArgb(idxx, idxx, idxx); // 設置為灰階色
                                        }

                                        bmp.Palette = palette;
                                        // Save the Bitmap to a file
                                        string filePath = @"D:\hhhhtemp.bmp";
                                        bmp.Save(filePath, ImageFormat.Bmp);

                                        Console.WriteLine("Image saved successfully at: " + filePath);
                                    } catch (Exception ex) {
                                        Console.WriteLine("Error saving the image: " + ex.Message);
                                    }
                                }

                                //for (int hidx = 0; hidx < length; hidx++) {
                                //    for (int wIdx = 0; wIdx < width; wIdx++) {
                                //        if (ranges[wIdx + width*hidx] != -32768) {
                                //            // x = xresolution * wIdx + x Offset
                                //            // z =  zresolution * ranges[wIdx + width*hidx] + z Offset
                                //        }
                                //    }
                                //}
                        } break;

                        case GoDataMessageType.SurfacePointCloud: {
                            GoSurfacePointCloudMsg surfaceMsg = (GoSurfacePointCloudMsg)dataObj;
                            context.xResolution = (double)surfaceMsg.XResolution / 1000000;
                            context.yResolution = (double)surfaceMsg.YResolution / 1000000;
                            context.zResolution = (double)surfaceMsg.ZResolution / 1000000;
                            context.xOffset = (double)surfaceMsg.XOffset / 1000;
                            context.yOffset = (double)surfaceMsg.YOffset / 1000;
                            context.zOffset = (double)surfaceMsg.ZOffset / 1000;

                            long surfacePointCount = surfaceMsg.Width * surfaceMsg.Length;
                            Console.WriteLine("Surface Point Cloud received:");
                            Console.WriteLine(" Buffer width: {0}", surfaceMsg.Width);
                            Console.WriteLine(" Buffer length: {0}", surfaceMsg.Length);

                            GoPoints[] points = new GoPoints[surfacePointCount];
                            SurfacePoint[] surfaceBuffer = new SurfacePoint[surfacePointCount];
                            int structSize = Marshal.SizeOf(typeof(GoPoints));
                            IntPtr pointsPtr = surfaceMsg.Data;

                            for (UInt32 array = 0; array < surfacePointCount; ++array) {
                                IntPtr incPtr = new IntPtr(pointsPtr.ToInt64() + array * structSize);
                                points[array] = (GoPoints)Marshal.PtrToStructure(incPtr, typeof(GoPoints));
                            }

                            for (UInt32 arrayIndex = 0; arrayIndex < surfacePointCount; ++arrayIndex) {
                                if (points[arrayIndex].x != -32768) {
                                    surfaceBuffer[arrayIndex].x = context.xOffset + context.xResolution * points[arrayIndex].x;
                                    surfaceBuffer[arrayIndex].y = context.yOffset + context.yResolution * points[arrayIndex].y;
                                    surfaceBuffer[arrayIndex].z = context.zOffset + context.zResolution * points[arrayIndex].z;
                                } else {
                                    surfaceBuffer[arrayIndex].x = -32768;
                                    surfaceBuffer[arrayIndex].y = -32768;
                                    surfaceBuffer[arrayIndex].z = -32768;
                                }
                            }
                        } break;

                        case GoDataMessageType.SurfaceIntensity: {
                            GoSurfaceIntensityMsg surfaceMsg = (GoSurfaceIntensityMsg)dataObj;
                            long width = surfaceMsg.Width;
                            long length = surfaceMsg.Length;
                            long bufferSize = width * length;
                            IntPtr bufferPointeri = surfaceMsg.Data;

                            Console.WriteLine("Surface Intensity received:");
                            Console.WriteLine(" Buffer width: {0}", width);
                            Console.WriteLine(" Buffer length: {0}", length);
                            byte[] ranges = new byte[bufferSize];
                            Marshal.Copy(bufferPointeri, ranges, 0, ranges.Length);
                        } break;
                    }  // end of switch (dataObj.MessageType) {
                }  // end of for (UInt32 i = 0; i < dataSet.Count; i++) {

                system.Stop();

                accelerator.Detach(sensor);
                accelerator.Stop();

                accelerator.Dispose();
            } catch (KException ex) {
                Console.WriteLine("Error: {0}", ex.ToString());
            }
            // wait for ENTER key
            Console.WriteLine("\nPress ENTER to continue");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }

            return 1;
        }
    }
}
