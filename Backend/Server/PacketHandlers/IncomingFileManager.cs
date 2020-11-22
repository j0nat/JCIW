using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using Networking.Data;
using Networking.Data.Packets;
using System;
using System.Collections.Generic;
using System.IO;

namespace Server.PacketHandlers
{
    /*
        Transfer code retrieved from NetworkComms example https://www.networkcomms.net/creating-a-wpf-file-transfer-application/
        on 07.10.2020 
    */
    public class IncomingFileManager
    {
        #region Private Fields

        private AccountManager accountManager;

        /// <summary>
        /// References to received files by remote ConnectionInfo
        /// </summary>
        Dictionary<ConnectionInfo, Dictionary<string, ReceivedFile>> receivedFilesDict = 
            new Dictionary<ConnectionInfo, Dictionary<string, ReceivedFile>>();

        /// <summary>
        /// Incoming partial data cache. Keys are ConnectionInfo, PacketSequenceNumber. Value is partial packet data.
        /// </summary>
        Dictionary<ConnectionInfo, Dictionary<long, byte[]>> incomingDataCache = 
            new Dictionary<ConnectionInfo, Dictionary<long, byte[]>>();

        /// <summary>
        /// Incoming sendInfo cache. Keys are ConnectionInfo, PacketSequenceNumber. Value is sendInfo.
        /// </summary>
        Dictionary<ConnectionInfo, Dictionary<long, SendInfo>> incomingDataInfoCache = 
            new Dictionary<ConnectionInfo, Dictionary<long, SendInfo>>();

        /// <summary>
        /// Object used for ensuring thread safety.
        /// </summary>
        object syncRoot = new object();
        #endregion

        private ModuleManager moduleManager;

        public IncomingFileManager(ModuleManager moduleManager, AccountManager accountManager)
        {
            this.moduleManager = moduleManager;
            this.accountManager = accountManager;

            //Trigger IncomingPartialFileData method if we receive a packet of type 'PartialFileData'
            NetworkComms.AppendGlobalIncomingPacketHandler<byte[]>("PartialFileData", IncomingPartialFileData);
            //Trigger IncomingPartialFileDataInfo method if we receive a packet of type 'PartialFileDataInfo'
            NetworkComms.AppendGlobalIncomingPacketHandler<SendInfo>("PartialFileDataInfo", IncomingPartialFileDataInfo);
        }

        /// <summary>
        /// Handles an incoming packet of type 'PartialFileData'
        /// </summary>
        /// <param name="header">Header associated with incoming packet</param>
        /// <param name="connection">The connection associated with incoming packet</param>
        /// <param name="data">The incoming data</param>
        private void IncomingPartialFileData(PacketHeader header, Connection connection, byte[] data)
        {
            // ########################################################################
            // This method requires authentication. 
            // If user is not authorized then send UnAuthorized and end method.
            if (!accountManager.Authorized(connection))
            {
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUnauthorized.ToString(), 1);

                return;
            }
            // ########################################################################

            try
            {
                SendInfo info = null;
                ReceivedFile file = null;

                //Perform this in a thread safe way
                lock (syncRoot)
                {
                    //Extract the packet sequence number from the header
                    //The header can also user defined parameters
                    long sequenceNumber = header.GetOption(PacketHeaderLongItems.PacketSequenceNumber);

                    if (incomingDataInfoCache.ContainsKey(connection.ConnectionInfo) && incomingDataInfoCache[connection.ConnectionInfo].ContainsKey(sequenceNumber))
                    {
                        //We have the associated SendInfo so we can add this data directly to the file
                        info = incomingDataInfoCache[connection.ConnectionInfo][sequenceNumber];
                        incomingDataInfoCache[connection.ConnectionInfo].Remove(sequenceNumber);

                        //Check to see if we have already received any files from this location
                        if (!receivedFilesDict.ContainsKey(connection.ConnectionInfo))
                            receivedFilesDict.Add(connection.ConnectionInfo, new Dictionary<string, ReceivedFile>());

                        //Check to see if we have already initialised this file
                        if (!receivedFilesDict[connection.ConnectionInfo].ContainsKey(info.Filename))
                        {
                            ReceivedFile newReceivedFile = new ReceivedFile(info.Filename, connection.ConnectionInfo, info.TotalBytes);

                            newReceivedFile.ReceivedFileFinishedEvent += NewReceivedFile_ReceivedFileFinishedEvent;
                            receivedFilesDict[connection.ConnectionInfo].Add(info.Filename, newReceivedFile);

                        }

                        file = receivedFilesDict[connection.ConnectionInfo][info.Filename];
                    }
                    else
                    {
                        //We do not yet have the associated SendInfo so we just add the data to the cache
                        if (!incomingDataCache.ContainsKey(connection.ConnectionInfo))
                            incomingDataCache.Add(connection.ConnectionInfo, new Dictionary<long, byte[]>());

                        incomingDataCache[connection.ConnectionInfo].Add(sequenceNumber, data);
                    }
                }

                //If we have everything we need we can add data to the ReceivedFile
                if (info != null && file != null && !file.IsCompleted)
                {
                    file.AddData(info.BytesStart, 0, data.Length, data);

                    //Perform a little clean-up
                    file = null;
                    data = null;
                    GC.Collect();
                }
                else if (info == null ^ file == null)
                    throw new Exception("Either both are null or both are set. Info is " + (info == null ? "null." : "set.") + " File is " + (file == null ? "null." : "set.") + " File is " + (file.IsCompleted ? "completed." : "not completed."));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //If an exception occurs we write to the log window and also create an error file
             //   AddLineToLog("Exception - " + ex.ToString());
           //     LogTools.LogException(ex, "IncomingPartialFileDataError");
            }
        }

        private void NewReceivedFile_ReceivedFileFinishedEvent(ReceivedFile file)
        {
            file.ReceivedFileFinishedEvent -= NewReceivedFile_ReceivedFileFinishedEvent;

            lock (syncRoot)
            {
                if (receivedFilesDict.ContainsKey(file.SourceInfo))
                {
                    Dictionary<string, ReceivedFile> connReceivedFileDict = receivedFilesDict[file.SourceInfo];
                    ReceivedFile connReceivedFile = connReceivedFileDict[file.Filename];

                    if (!Directory.Exists("plugins"))
                    {
                        Directory.CreateDirectory("plugins");
                    }

                    string onlyFileName = Path.GetFileNameWithoutExtension(connReceivedFile.Filename);
                    string randomGuid = Guid.NewGuid().ToString().Substring(0, 5);
                    string saveFilePath = Path.Combine("plugins", onlyFileName + randomGuid + ".dll");
                    connReceivedFile.SaveFileToDisk(saveFilePath);
                    connReceivedFile.Close();

                    lock (receivedFilesDict)
                    {
                        receivedFilesDict.Remove(file.SourceInfo);
                    }

                    moduleManager.RegisterModule(saveFilePath);
                }
            }
        }

        /// <summary>
        /// Handles an incoming packet of type 'PartialFileDataInfo'
        /// </summary>
        /// <param name="header">Header associated with incoming packet</param>
        /// <param name="connection">The connection associated with incoming packet</param>
        /// <param name="data">The incoming data automatically converted to a SendInfo object</param>
        private void IncomingPartialFileDataInfo(PacketHeader header, Connection connection, SendInfo info)
        {
            // ########################################################################
            // This method requires authentication. 
            // If user is not authorized then send UnAuthorized and end method.
            if (!accountManager.Authorized(connection))
            {
                TCPConnection.GetConnection(connection.ConnectionInfo).SendObject(
                    PacketName.ReUnauthorized.ToString(), 1);

                return;
            }
            // ########################################################################

            try
            {
                byte[] data = null;
                ReceivedFile file = null;

                //Perform this in a thread safe way
                lock (syncRoot)
                {
                    //Extract the packet sequence number from the header
                    //The header can also user defined parameters
                    long sequenceNumber = info.PacketSequenceNumber;

                    if (incomingDataCache.ContainsKey(connection.ConnectionInfo) && incomingDataCache[connection.ConnectionInfo].ContainsKey(sequenceNumber))
                    {
                        //We already have the associated data in the cache
                        data = incomingDataCache[connection.ConnectionInfo][sequenceNumber];
                        incomingDataCache[connection.ConnectionInfo].Remove(sequenceNumber);

                        //Check to see if we have already received any files from this location
                        if (!receivedFilesDict.ContainsKey(connection.ConnectionInfo))
                            receivedFilesDict.Add(connection.ConnectionInfo, new Dictionary<string, ReceivedFile>());

                        //Check to see if we have already initialised this file
                        if (!receivedFilesDict[connection.ConnectionInfo].ContainsKey(info.Filename))
                        {
                            ReceivedFile newReceivedFile = new ReceivedFile(info.Filename, connection.ConnectionInfo, info.TotalBytes);

                            newReceivedFile.ReceivedFileFinishedEvent += NewReceivedFile_ReceivedFileFinishedEvent;
                            receivedFilesDict[connection.ConnectionInfo].Add(info.Filename, newReceivedFile);

                           // receivedFilesDict[connection.ConnectionInfo].Add(info.Filename, new ReceivedFile(info.Filename, connection.ConnectionInfo, info.TotalBytes));
                          //  AddNewReceivedItem(receivedFilesDict[connection.ConnectionInfo][info.Filename]);
                        }

                        file = receivedFilesDict[connection.ConnectionInfo][info.Filename];
                    }
                    else
                    {
                        //We do not yet have the necessary data corresponding with this SendInfo so we add the
                        //info to the cache
                        if (!incomingDataInfoCache.ContainsKey(connection.ConnectionInfo))
                            incomingDataInfoCache.Add(connection.ConnectionInfo, new Dictionary<long, SendInfo>());

                        incomingDataInfoCache[connection.ConnectionInfo].Add(sequenceNumber, info);
                    }
                }

                //If we have everything we need we can add data to the ReceivedFile
                if (data != null && file != null && !file.IsCompleted)
                {
                    file.AddData(info.BytesStart, 0, data.Length, data);

                    //Perform a little clean-up
                    file = null;
                    data = null;
                    GC.Collect();
                }
                else if (data == null ^ file == null)
                    throw new Exception("Either both are null or both are set. Data is " + (data == null ? "null." : "set.") + " File is " + (file == null ? "null." : "set.") + " File is " + (file.IsCompleted ? "completed." : "not completed."));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //If an exception occurs we write to the log window and also create an error file
                //    AddLineToLog("Exception - " + ex.ToString());
                //   LogTools.LogException(ex, "IncomingPartialFileDataInfo");
            }
        }
    }
}
