using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.Annotations;
using Java.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace MineSweeper
{
    class MyScaleListener : Java.Lang.Object, ScaleGestureDetector.IOnScaleGestureListener
    {
        private TableLayout tableLayout;
        float finalScaleX = 1;
        float finalScaleY = 1;

        // Constructor that takes a TableLayout as a parameter
        public MyScaleListener(TableLayout tableLayout)
        {
            this.tableLayout = tableLayout;
        }

        // Implement the onScaleBegin method to set the initial state of the scale gesture
        public bool OnScaleBegin(ScaleGestureDetector detector)
        {
            // Set the initial scale factor to 1
            tableLayout.ScaleX = finalScaleX;
            tableLayout.ScaleY = finalScaleY;
            // Return true to indicate that the gesture was handled
            return true;
        }

        // Implement the onScale method to update the scale factor of the TableLayout
        public bool OnScale(ScaleGestureDetector detector)
        {
            // Update the scale factor of the TableLayout based on the scale factor of the gesture
            if (tableLayout.ScaleX >= 1 && tableLayout.ScaleX <= 2)
            {
                tableLayout.ScaleX *= detector.ScaleFactor;
                tableLayout.ScaleY *= detector.ScaleFactor;
            }
            else if(tableLayout.ScaleX >= 2 && detector.ScaleFactor < 1)
            {
                tableLayout.ScaleX *= detector.ScaleFactor;
                tableLayout.ScaleY *= detector.ScaleFactor;
            }
            else if(tableLayout.ScaleX <= 1 && detector.ScaleFactor > 1)
            {
                tableLayout.ScaleX *= detector.ScaleFactor;
                tableLayout.ScaleY *= detector.ScaleFactor;
            }

            // Return true to indicate that the gesture was handled
            return true;
        }

        // Implement the onScaleEnd method to save the final scale factor of the gesture
        public void OnScaleEnd(ScaleGestureDetector detector)
        {
            // Save the final scale factor of the gesture
            finalScaleX = tableLayout.ScaleX;
            finalScaleY = tableLayout.ScaleY;
        }
    }
}