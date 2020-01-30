﻿using System.Drawing;
using EuclideanFilter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LearningFoundation;
using System.IO;
using System;
using Microsoft.VisualStudio.Imaging;
using System.Drawing.Imaging;
using DocumentFormat.OpenXml.Drawing;
using System.Runtime.InteropServices;

namespace EuclideanColorFilterTests
{
    public class BitmapLocker : IDisposable
    {
        //private properties
        Bitmap _bitmap = null;
        BitmapData _bitmapData = null;
        private byte[] _imageData = null;

        //public properties
        public bool IsLocked { get; set; }
        public IntPtr IntegerPointer { get; private set; }
        public int Width
        {
            get
            {
                if (IsLocked == false) throw new InvalidOperationException("not locked");
                return _bitmapData.Width;
            }
        }
        public int Height
        {
            get
            {
                if (IsLocked == false) throw new InvalidOperationException("not locked");
                return _bitmapData.Height;
            }
        }
        public int Stride
        {
            get
            {
                if (IsLocked == false) throw new InvalidOperationException("not locked");
                return _bitmapData.Stride;
            }
        }
        public int ColorDepth
        {
            get
            {
                if (IsLocked == false) throw new InvalidOperationException("not locked");
                return Bitmap.GetPixelFormatSize(_bitmapData.PixelFormat);
            }
        }
        public int Channels
        {
            get
            {
                if (IsLocked == false) throw new InvalidOperationException("not locked");
                return ColorDepth / 8;
            }
        }
        public int PaddingOffset
        {
            get
            {
                if (IsLocked == false) throw new InvalidOperationException("not locked");
                return _bitmapData.Stride - (_bitmapData.Width * Channels);
            }
        }
        public PixelFormat ImagePixelFormat
        {
            get
            {
                if (IsLocked == false) throw new InvalidOperationException("not locked");
                return _bitmapData.PixelFormat;
            }
        }
        public bool IsGrayscale
        {
            get
            {
                if (IsLocked == false) throw new InvalidOperationException("not locked");
                return Grayscale.IsGrayscale(_bitmap);
            }
        }

        //Constructor
        public BitmapLocker(Bitmap source)
        {
            IsLocked = false;
            IntegerPointer = IntPtr.Zero;
            this._bitmap = source;
        }

        /// Lock bitmap
        public void Lock()
        {
            if (IsLocked == false)
            {
                try
                {
                    // Lock bitmap (so that no movement of data by .NET framework) and return bitmap data
                    _bitmapData = _bitmap.LockBits(
                           new System.Drawing.Rectangle(0, 0, _bitmap.Width, _bitmap.Height),
                           ImageLockMode.ReadWrite,
                           _bitmap.PixelFormat);

                    // Create byte array to copy pixel values
                    int noOfBytesNeededForStorage = Math.Abs(_bitmapData.Stride) * _bitmapData.Height;
                    _imageData = new byte[noOfBytesNeededForStorage];

                    IntegerPointer = _bitmapData.Scan0;

                    // Copy data from IntegerPointer to _imageData
                    Marshal.Copy(IntegerPointer, _imageData, 0, _imageData.Length);

                    IsLocked = true;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                throw new Exception("Bitmap is already locked.");
            }
        }

        /// Unlock bitmap
        public void Unlock()
        {
            if (IsLocked == true)
            {
                try
                {
                    // Copy data from _imageData to IntegerPointer
                    Marshal.Copy(_imageData, 0, IntegerPointer, _imageData.Length);

                    // Unlock bitmap data
                    _bitmap.UnlockBits(_bitmapData);

                    IsLocked = false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                throw new Exception("Bitmap is not locked.");
            }
        }

        public Color GetPixel(int x, int y)
        {
            Color clr = Color.Empty;

            // Get color components count
            int cCount = ColorDepth / 8;

            // Get start index of the specified pixel
            int i = (Stride > 0 ? y : y - Height + 1) * Stride + x * cCount;

            int dataLength = _imageData.Length - cCount;

            if (i > dataLength)
            {
                throw new IndexOutOfRangeException();
            }

            if (ColorDepth == 32) // For 32 bpp get Red, Green, Blue and Alpha
            {
                byte b = _imageData[i];
                byte g = _imageData[i + 1];
                byte r = _imageData[i + 2];
                byte a = _imageData[i + 3]; // a
                clr = Color.FromArgb(a, r, g, b);
            }
            if (ColorDepth == 24) // For 24 bpp get Red, Green and Blue
            {
                byte b = _imageData[i];
                byte g = _imageData[i + 1];
                byte r = _imageData[i + 2];
                clr = Color.FromArgb(r, g, b);
            }
            if (ColorDepth == 8)
            // For 8 bpp get color value (Red, Green and Blue values are the same)
            {
                byte c = _imageData[i];
                clr = Color.FromArgb(c, c, c);
            }
            return clr;
        }

        public void SetPixel(int x, int y, Color color)
        {

            if (!IsLocked) throw new Exception();

            // Get color components count
            int cCount = ColorDepth / 8;

            // Get start index of the specified pixel
            int i = (Stride > 0 ? y : y - Height + 1) * Stride + x * cCount;

            try
            {
                if (ColorDepth == 32) // For 32 bpp set Red, Green, Blue and Alpha
                {
                    _imageData[i] = color.B;
                    _imageData[i + 1] = color.G;
                    _imageData[i + 2] = color.R;
                    _imageData[i + 3] = color.A;
                }
                if (ColorDepth == 24) // For 24 bpp set Red, Green and Blue
                {
                    _imageData[i] = color.B;
                    _imageData[i + 1] = color.G;
                    _imageData[i + 2] = color.R;
                }
                if (ColorDepth == 8)
                // For 8 bpp set color value (Red, Green and Blue values are the same)
                {
                    _imageData[i] = color.B;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("(" + x + ", " + y + "), " + _imageData.Length + ", " + ex.Message + ", i=" + i);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                _bitmap = null;
                _bitmapData = null;
                _imageData = null;
                IntegerPointer = IntPtr.Zero;
            }
        }
    }
}
