using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NewEventLogDLL;
using VehicleBulkToolsDLL;
using VehicleMainDLL;
using ToolCategoryDLL;

namespace LoadBulkTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleBulkToolsClass TheVehicleBulkToolsClass = new VehicleBulkToolsClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        ToolCategoryClass TheToolCategoryClass = new ToolCategoryClass();

        FindActiveVehicleMainDataSet ThefindActiveVehicleMainDataSet = new FindActiveVehicleMainDataSet();
        FindVehicleBulkToolByVehicleNumberDataSet TheFindVehicleBulkToolByVehicleNumberDataSet = new FindVehicleBulkToolByVehicleNumberDataSet();
        FindSortedToolCategoryDataSet TheFindSortedToolCategoryDataSet = new FindSortedToolCategoryDataSet();

        int gintConeID;
        int gintSignID;
        int gintFireExtinguisherID;
        int gintFirstAidKitID;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void mitCreateHelpDeskTicket_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpDeskTickets();
        }

        private void mitHelpSite_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.LaunchHelpSite();
        }

        private void mitClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheFindSortedToolCategoryDataSet = TheToolCategoryClass.FindSortedToolCategory();

                intNumberOfRecords = TheFindSortedToolCategoryDataSet.FindSortedToolCategory.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if (TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intCounter].ToolCategory == "CONE")
                        gintConeID = TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intCounter].CategoryID;
                    else if (TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intCounter].ToolCategory == "SIGN")
                        gintSignID = TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intCounter].CategoryID;
                    else if (TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intCounter].ToolCategory == "FIRE EXTINGUISHER")
                        gintFireExtinguisherID = TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intCounter].CategoryID;
                    else if (TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intCounter].ToolCategory == "FIRST AID KIT")
                        gintFirstAidKitID = TheFindSortedToolCategoryDataSet.FindSortedToolCategory[intCounter].CategoryID;
                }

                ThefindActiveVehicleMainDataSet = TheVehicleMainClass.FindActiveVehicleMain();

                dgrResults.ItemsSource = ThefindActiveVehicleMainDataSet.FindActiveVehicleMain;
            }
            catch(Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Load Bulk Tools // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void mitProcess_Click(object sender, RoutedEventArgs e)
        {
            int intVehicleCounter;
            int intVehicleNumberOfRecords;
            int intToolCounter;
            int intVehicleID;
            bool blnFatalError;

            try
            {

                intVehicleNumberOfRecords = ThefindActiveVehicleMainDataSet.FindActiveVehicleMain.Rows.Count - 1;

                for (intVehicleCounter = 0; intVehicleCounter <= intVehicleNumberOfRecords; intVehicleCounter ++)
                {
                    intVehicleID = ThefindActiveVehicleMainDataSet.FindActiveVehicleMain[intVehicleCounter].VehicleID;

                    for (intToolCounter = 0; intToolCounter < 4; intToolCounter++)
                    {
                        if (intToolCounter == 0)
                            blnFatalError = TheVehicleBulkToolsClass.InsertVehicleBulkTools(intVehicleID, gintConeID, 0);
                        else if(intToolCounter == 1)
                            blnFatalError = TheVehicleBulkToolsClass.InsertVehicleBulkTools(intVehicleID, gintSignID, 0);
                        else if (intToolCounter == 2)
                            blnFatalError = TheVehicleBulkToolsClass.InsertVehicleBulkTools(intVehicleID, gintFireExtinguisherID, 1);
                        else if (intToolCounter == 3)
                            blnFatalError = TheVehicleBulkToolsClass.InsertVehicleBulkTools(intVehicleID, gintFirstAidKitID, 1);
                    }
                }

                TheMessagesClass.ErrorMessage("The Tools Have Been Imported");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Load Bulk Tools // Process Menu Item " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
