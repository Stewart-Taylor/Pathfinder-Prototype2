using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pathfinder_Prototype_2
{
    public partial class MainWindow : Window
    {
        PathfinderController pathfinderController = new PathfinderController();


        public MainWindow()
        {
            InitializeComponent();

            setUp();
        }

        private void setUp()
        {
            cmb_elevationMap.SelectedIndex = 0;
            cmb_slopeAlgorithm.SelectedIndex = 2;
            cmb_pathAlgorithm.SelectedIndex = 0;

        
        }

        private String getElevationMapPath()
        {
            String path = "Models//model1_w512_h0.1_v0.01.ppm";

            if (cmb_elevationMap.SelectedIndex == 0)
            {
                path = "Models//model1_w512_h0.1_v0.01.ppm";
            }
            else if (cmb_elevationMap.SelectedIndex == 1)
            {
                path = "Models//model2_w1024_h0.1_v0.01.ppm";
            }

            return path;
        }

        private String getSlopeType()
        {
            string slopeType = "";

            System.Windows.Controls.ComboBoxItem curItem = ((System.Windows.Controls.ComboBoxItem)cmb_slopeAlgorithm.SelectedItem);

            if (curItem != null)
            {
                slopeType = curItem.Content.ToString();
            }

            return slopeType;
        }


        private int getSectorSize()
        {
            int size = 10;

            try
            {
                size = int.Parse(txt_sectorSize.Text);
            }
            catch (Exception )
            {
                MessageBox.Show("Please enter a valid positive integer value");
                
            }
            return size;
        }

        private void btn_simulate_Click(object sender, RoutedEventArgs e)
        {

            pathfinderController.loadHazardModel(getSectorSize());
            

            int startX = 0;
            int startY = 0;
            int targetX = 22;
            int targetY = 30;

            try
            {
                startX = int.Parse(txt_startX.Text);
                startY = int.Parse(txt_startY.Text);
                targetX = int.Parse(txt_endX.Text);
                targetY = int.Parse(txt_endY.Text);
            }
            catch (Exception)
            {

            }

            pathfinderController.generatePath(startX, startY, targetX, targetY);
            img_main.Source = pathfinderController.getPathImage();
        }

        private void elvmapchanged(object sender, SelectionChangedEventArgs e)
        {
            pathfinderController.loadElevationModel(getElevationMapPath());
            pathfinderController.loadSlopeModel(getSlopeType());
        }

        private void cmb_slopeAlgorithm_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            pathfinderController.loadSlopeModel(getSlopeType());
        }


    }
}
