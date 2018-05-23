using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bluetooth_List_Forms
{
    public partial class Form1 : Form
    {

        
        public Form1()
        {
            InitializeComponent();
        }

        private BluetoothDeviceInfo[] devices;  
        // Contains list of all devices -> newly discovered + previous connected ;

        #region Find Devices
        private void button1_Click(object sender, EventArgs e)
        {
            listViewDevices.Items.Clear();
            BluetoothClient client = new BluetoothClient(); //Create an Instance 
            //var devicesList = new List<Devices>();
            devices = client.DiscoverDevices();         // find all devices in vicinity + also previosusly remembered device
            foreach (var device in devices)
            {

                var row = new string[] { device.DeviceName, device.DeviceAddress.ToString() };
                var lvi = new ListViewItem(row);
                listViewDevices.Items.Add(lvi);         // Add them to ListView
            }
            
        }
        #endregion

        #region Called When Form is loaded
        static int count = 0;          //TimePass XD
        private void Form1_Load(object sender, EventArgs e)
        {
            count++;
        }
        #endregion

        //int index = listViewDevices.SelectedIndices[0];


        #region Connect
        private void button2_Click(object sender, EventArgs e)
        {
            if (listViewDevices.SelectedItems.Count > 0)
            {
                EventHandler<BluetoothWin32AuthenticationEventArgs> authHandler = new EventHandler<BluetoothWin32AuthenticationEventArgs>(handleAuthRequests);
                BluetoothWin32Authentication authenticator = new BluetoothWin32Authentication(authHandler);

                BluetoothDeviceInfo selectedDevice = devices[listViewDevices.SelectedIndices[0]];
                if (MessageBox.Show(String.Format("Would you like to attempt to pair with {0}?", selectedDevice.DeviceName), "Pair Device", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (BluetoothSecurity.PairRequest(selectedDevice.DeviceAddress, null))
                    {
                        MessageBox.Show("We paired!");
                    }
                    else
                    {
                        MessageBox.Show("Failed to pair!");
                    }
                }
            }
            else
            {
                MessageBox.Show("Select a Device to Pair");
            }
            
        }

        



        private void handleAuthRequests(object sender, BluetoothWin32AuthenticationEventArgs e)
        {
            switch (e.AuthenticationMethod)
            {
                case BluetoothAuthenticationMethod.Legacy:
                    MessageBox.Show("Legacy Authentication");
                    break;

                case BluetoothAuthenticationMethod.OutOfBand:
                    MessageBox.Show("Out of Band Authentication");
                    break;

                case BluetoothAuthenticationMethod.NumericComparison:
                    if (e.JustWorksNumericComparison == true)
                    {
                        MessageBox.Show("Just Works Numeric Comparison");
                    }
                    else
                    {
                        MessageBox.Show("Show User Numeric Comparison");
                        if (MessageBox.Show(e.NumberOrPasskeyAsString, "Pair Device", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            e.Confirm = true;
                        }
                        else
                        {
                            e.Confirm = false;
                        }
                    }
                    break;

                case BluetoothAuthenticationMethod.PasskeyNotification:
                    MessageBox.Show("Passkey Notification");
                    break;

                case BluetoothAuthenticationMethod.Passkey:
                    MessageBox.Show("Passkey");
                    break;

                default:
                    MessageBox.Show("Event handled in some unknown way");
                    break;

            }
        }

        #endregion

        #region Send File (Don't forget to change location in request.ReadFile(**Here**) in sendFile() method )
        int selected;
        private Thread thrsend;
        private void button3_Click(object sender, EventArgs e)
        {
            if(this.listViewDevices.SelectedIndices[0] == -1)
            {
                MessageBox.Show("Please select a device.");
                return;
            }
            selected = listViewDevices.SelectedIndices[0];
            this.thrsend = new Thread(new ThreadStart(sendfile));
            this.thrsend.Start();
        }

        private void sendfile()
        {
            int index = selected;
            InTheHand.Net.BluetoothAddress address = devices[index].DeviceAddress;
            System.Uri uri = new Uri("obex://" + address.ToString() + "/" + "sample.txt");  //Change it to your file name
            ObexWebRequest request = new ObexWebRequest(uri);
            request.ReadFile("c:\\users\\chinmay\\sample.txt"); // Chnage it to your File Path
            ObexWebResponse response = (ObexWebResponse)request.GetResponse();
            response.Close();
        }

        #endregion

        #region Exit
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
