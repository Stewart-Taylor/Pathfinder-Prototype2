﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;


namespace Pathfinder_Prototype_2
{
    class PathfinderController
    {
        private ElevationLoader elevationLoader;
        private SlopeModel slopeModel;
        private HazardModel hazardModel;
        private Pathfinder pathfinder;

        private Random r = new Random();

        private bool stepTraverseStarted = false;

        private Vehicle rover;
        private int steps;

        private ImageSource imageSource;

        private String timeTaken;

        #region GET

        public String getTimeTaken()
        {
            return timeTaken;
        }

        public int getSteps()
        {
            return steps;
        }

        #endregion


        public PathfinderController()
        {
          
        }

        public void loadElevationModel(String path)
        {
            elevationLoader = new ElevationLoader(path);
        }


        public void loadSlopeModel(String slopeType)
        {
            slopeModel = new SlopeModel(elevationLoader.getHeightMap(), slopeType);
        }


        public void loadHazardModel(int chunkSize)
        {
            try
            {
                hazardModel = new HazardModel(slopeModel.getSlopeModel(), chunkSize);
            }
            catch (Exception e) { }
        }

        public void generatePath(int startX, int startY, int targetX, int targetY)
        {
            if ((startX + startY + targetX + targetY) == 0)
            {
                pathfinder = new Pathfinder(hazardModel.getHazardModel() , hazardModel.hazardModelImage, 0, 0, r.Next(hazardModel.getHazardModel().GetLength(0)), r.Next(hazardModel.getHazardModel().GetLength(1)));
            }
            else
            {
                try
                {
                    pathfinder = new Pathfinder(hazardModel.getHazardModel(), hazardModel.hazardModelImage, startX, startY, targetX, targetY);
                }
                catch (Exception) { }
            }
        }


        public void startSimulationKnownMap(int startX, int startY, int endX, int endY)
        {
            Pathfinder pathfinder = new Pathfinder(hazardModel.getHazardModel() , hazardModel.hazardModelImage, startX, startY, endX, endY);

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            pathfinder.findPath(startX, startY, endX, endY);

            sw.Stop();

            pathfinder.generatePathImage();
            steps = pathfinder.getPath().Count;
            imageSource = pathfinder.getPathImage();

            timeTaken = sw.Elapsed.TotalSeconds + " Seconds";

            stepTraverseStarted = false;
        }


        private void generatePathImage()
        {


        }

        public void startSimulation(int startX , int startY , int endX , int endY)
        {
            rover = new Vehicle(hazardModel.getHazardModel() , hazardModel.hazardModelImage);

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            rover.traverseMap(startX, startY, endX, endY);

            sw.Stop();

            rover.generatePathImage();
            steps = rover.getSteps();
            imageSource = rover.getPathImage();

            timeTaken = sw.Elapsed.TotalSeconds + " Seconds";

            stepTraverseStarted = false;
        }


        public void startSimulationDSTAR(int startX, int startY, int endX, int endY)
        {
            rover = new Vehicle(hazardModel.getHazardModel() , hazardModel.hazardModelImage);

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            rover.traverseMapDstar(startX, startY, endX, endY);

            sw.Stop();

            rover.generatePathImage();
            steps = rover.getSteps();
            imageSource = rover.getPathImage();

            timeTaken = sw.Elapsed.TotalSeconds + " Seconds";

            stepTraverseStarted = false;
        }



        public void nextStep(int startX , int startY , int endX , int endY)
        {
            if (stepTraverseStarted == false)
            {
                rover = new Vehicle(hazardModel.getHazardModel() , hazardModel.hazardModelImage);
                rover.startTraverse(startX, startY, endX, endY);
                stepTraverseStarted = true;
            }
            else
            {
                rover.traverseMapStep();
                imageSource = rover.getPathImage();
            }
        }


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
          //  rover = new Vehicle(hazardModel.getHazardModel());
         //   rover.traverseMap(startX, startY, endX, endY);
        }


        public ImageSource getElevationModelImage()
        {
            return elevationLoader.getImageSource();
        }

        public float[,] getElevationModel()
        {
            return elevationLoader.getHeightMap();
        }

        public float[,] getSlopeModel()
        {
            return slopeModel.getSlopeModel();
        }

        public float[,] getHazardModel()
        {
            return hazardModel.getHazardModel();
        }

        public List<PathNode> getPathModel()
        {
            return pathfinder.getPath();
        }

        public ImageSource getSlopeModelImage()
        {
            return slopeModel.getSlopeModelImage();
        }

        public ImageSource getHazardModelImage()
        {
            return hazardModel.getHazardModelImage();
        }

        public ImageSource getPathImage()
        {
            return pathfinder.getPathImage();
        }


        public ImageSource getVehicleImage()
        {
            return imageSource;
        }

    }
}