namespace RoboHand.Aruco;
using System;
using System.Drawing;
using Emgu.CV; // the mom
using Emgu.CV.Aruco; // the hero
using Emgu.CV.CvEnum; // the book
using Emgu.CV.Structure; // the storage
using Emgu.CV.Util; // the side kick

public class ArucoDetection
{
    public static IEnumerable<byte[]> Start()
    {
        #region Initialize video capture object on default webcam (0)
        // Instantiate a webcam abstraction
        VideoCapture capture;
        capture = new VideoCapture(0);
        #endregion

        #region Initialize and save Aruco dictionary and gridboard
        int markersX = 4;
        int markersY = 4;
        int markersLength = 80;
        int markersSeparation = 30;
        Dictionary ArucoDict = new Dictionary(Dictionary.PredefinedDictionaryName.Dict4X4_100); // bits x bits (per marker) _ number of markers in dict
        GridBoard ArucoBoard = new GridBoard(markersX, markersY, markersLength, markersSeparation, ArucoDict);
        #endregion

        #region Initialize Aruco parameters for markers detection
        DetectorParameters ArucoParameters = new DetectorParameters();
        ArucoParameters = DetectorParameters.GetDefault();
        #endregion

        #region Infinite loop processing the image
        while (true)
        {
            #region Capture a frame with webcam
            Mat frame = new Mat();
            frame = capture.QueryFrame();
            #endregion
            if (!frame.IsEmpty)
            {
                #region Detect markers on last retrieved frame
                VectorOfInt ids = new VectorOfInt(); // name/id of the detected markers
                VectorOfVectorOfPointF corners = new VectorOfVectorOfPointF(); // corners of the detected marker
                VectorOfVectorOfPointF rejected = new VectorOfVectorOfPointF(); // rejected contours
                ArucoInvoke.DetectMarkers(frame, ArucoDict, corners, ids, ArucoParameters, rejected);
                #endregion

                // If we detected at least one marker
                if (ids.Size > 0)
                {
                    #region Draw detected markers
                    ArucoInvoke.DrawDetectedMarkers(frame, corners, ids, new MCvScalar(255, 0, 255));
                    #endregion

                    #region Estimate pose for each marker using camera calibration matrix and distortion coefficents
                    /*Mat rvecs = new Mat(); // rotation vector
                    Mat tvecs = new Mat(); // translation vector
                    ArucoInvoke.EstimatePoseSingleMarkers(corners, markersLength, cameraMatrix, distortionMatrix, rvecs, tvecs);*/
                    #endregion
                    
                }

                yield return frame.ToImage<Bgr, byte>().ToJpegData();
                Thread.Sleep(100);
            }
        }
        #endregion

    }
}