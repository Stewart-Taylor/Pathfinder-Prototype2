﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;


namespace Pathfinder_Prototype_2
{
    class PathfinderController
    {
        private ElevationLoader elevationLoader;
        private SlopeModel slopeModel;
        private HazardModel hazardModel;
        private Pathfinder pathfinder;

        private Random r = new Random();


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
                pathfinder = new Pathfinder(hazardModel.getHazardModel(), 0, 0, r.Next(hazardModel.getHazardModel().GetLength(0)), r.Next(hazardModel.getHazardModel().GetLength(1)));
            }
            else
            {
                try
                {
                    pathfinder = new Pathfinder(hazardModel.getHazardModel(), startX, startY, targetX, targetY);
                }
                catch (Exception e) { }
            }
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

    }
}