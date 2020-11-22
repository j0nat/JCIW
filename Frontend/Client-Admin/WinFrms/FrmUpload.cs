using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Tools;
using Networking.Data.Packets;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using JCIW.Module;

namespace Client_Admin.WinFrms
{
    public partial class FrmUpload : Form
    {
        private bool isBusy;

        public FrmUpload()
        {
            this.isBusy = false;

            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult fileDialogResult = openFileDialog1.ShowDialog();

            if (fileDialogResult == DialogResult.OK || fileDialogResult == DialogResult.Yes)
            {
                string filePath = openFileDialog1.FileName;

                if (File.Exists(filePath))
                {
                    if (VerifyModule(filePath))
                    {
                        string fileName = Path.GetFileName(filePath);

                        label1.Text = "Ready to upload.\r\n" + "File: " + fileName;
                        btnUpload.Enabled = true;
                    }
                    else
                    {

                        label1.Text = "Selected file not a valid module.";
                        btnUpload.Enabled = false;
                    }
                }
                else
                {
                    label1.Text = "Invalid File.";
                }
            }
            else
            {
                label1.Text = "Browse Canceled.";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        /*
            Transfer code partially retrieved from NetworkComms example https://www.networkcomms.net/creating-a-wpf-file-transfer-application/
            on 07.10.2020 
        */
        private void btnUpload_Click(object sender, EventArgs e)
        {
            string filename = openFileDialog1.FileName;

            if (File.Exists(filename))
            {
                btnUpload.Enabled = false;
                btnBrowse.Enabled = false;
                this.isBusy = true;

                progressBar1.Value = (int)0;

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        //Create a fileStream from the selected file
                        FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);

                        //Wrap the fileStream in a threadSafeStream so that future operations are thread safe
                        StreamTools.ThreadSafeStream safeStream = new StreamTools.ThreadSafeStream(stream);

                        //Get the filename without the associated path information
                        string shortFileName = System.IO.Path.GetFileName(filename);

                        //Parse the remote connectionInfo
                        //We have this in a separate try catch so that we can write a clear message to the log window
                        //if there are problems

                        //Get a connection to the remote side
                        Connection connection = AdminNetwork.Connection();

                        //Break the send into 20 segments. The less segments the less overhead 
                        //but we still want the progress bar to update in sensible steps
                        long sendChunkSizeBytes = (long)(stream.Length / 20.0) + 1;

                        //Limit send chunk size to 500MB
                        long maxChunkSizeBytes = 500L * 1024L * 1024L;
                        if (sendChunkSizeBytes > maxChunkSizeBytes) sendChunkSizeBytes = maxChunkSizeBytes;

                        long totalBytesSent = 0;
                        do
                        {
                            //Check the number of bytes to send as the last one may be smaller
                            long bytesToSend = (totalBytesSent + sendChunkSizeBytes < stream.Length ? sendChunkSizeBytes : stream.Length - totalBytesSent);

                            //Wrap the threadSafeStream in a StreamSendWrapper so that we can get NetworkComms.Net
                            //to only send part of the stream.
                            StreamTools.StreamSendWrapper streamWrapper = new StreamTools.StreamSendWrapper(safeStream, totalBytesSent, bytesToSend);

                            //We want to record the packetSequenceNumber
                            long packetSequenceNumber;
                            //Send the select data
                            connection.SendObject("PartialFileData", streamWrapper, NetworkComms.DefaultSendReceiveOptions, out packetSequenceNumber);
                            //Send the associated SendInfo for this send so that the remote can correctly rebuild the data
                            connection.SendObject("PartialFileDataInfo", new SendInfo(shortFileName, stream.Length, totalBytesSent, packetSequenceNumber), NetworkComms.DefaultSendReceiveOptions);

                            totalBytesSent += bytesToSend;

                            //Update the GUI with our send progress
                            UpdateSendProgress(shortFileName, (double)totalBytesSent / stream.Length);
                        } while (totalBytesSent < stream.Length);

                        //Clean up any unused memory
                        GC.Collect();

                        UpdateSendProgress(shortFileName, 100);
                        //   AddLineToLog("Completed file send to '" + connection.ConnectionInfo.ToString() + "'.");
                    }
                    catch (CommunicationException)
                    {
                        //If there is a communication exception then we just write a connection
                        //closed message to the log window
                        //   AddLineToLog("Failed to complete send as connection was closed.");
                        MessageBox.Show("Failed to complete send as connection was closed.");
                    }
                    catch (Exception ex)
                    {
                        //If we get any other exception which is not an InvalidDataException
                        //we log the error
                        /* if (!windowClosing && ex.GetType() != typeof(InvalidDataException))
                         {
                             AddLineToLog(ex.Message.ToString());
                             LogTools.LogException(ex, "SendFileError");
                         }*/

                        MessageBox.Show(ex.ToString());
                    }
                });
            }
        }

        public bool VerifyModule(string fileName)
        {
            bool returnValue = false;

            ModuleHeader serviceHeader = ModuleHeaderReader.ServiceHeader(Path.GetDirectoryName(fileName), fileName);
            ModuleHeader pluginHeader = ModuleHeaderReader.AppHeader(Path.GetDirectoryName(fileName), fileName);

            if (pluginHeader != null || serviceHeader != null)
            {
                returnValue = true;
            }

            return returnValue;
        }

        private void UpdateSendProgress(string fileName, double progress)
        {
            btnUpload.Invoke(new Action(() =>
            {
                progressBar1.Value = (int)progress;

                if (progress != 100)
                {
                    label1.Text = "Uploading file " + fileName;
                }
                else
                {
                    label1.Text = "Finished uploading file " + fileName;
                    btnUpload.Enabled = false;
                    btnBrowse.Enabled = true;
                    btnCancel.Text = "Done";
                    this.isBusy = false;
                }
            }));
        }

        private void FrmUpload_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isBusy)
            {
                e.Cancel = true;
            }
        }
    }
}
