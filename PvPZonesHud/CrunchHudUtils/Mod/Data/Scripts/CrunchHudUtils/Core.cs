using System;
using System.Collections.Generic;
using System.Linq;
using PvPZonesHud.Mod.Data.Scripts.CrunchHudUtils;
using Sandbox.Definitions;
using Sandbox.Game;
using Sandbox.Game.World;
using Sandbox.ModAPI;
using SpaceEngineers.Game.ModAPI;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.Scripting;
using VRage.Utils;

namespace Crunch
{
	// This object is always present, from the world load to world unload.
	// The MyUpdateOrder arg determines what update overrides are actually called.
	[MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation | MyUpdateOrder.AfterSimulation)]
	public class CrunchChat : MySessionComponentBase
	{

		private bool _isInitialized; // Is this instance is initialized
		public bool _isClientRegistered;
		public bool _isServerRegistered;

		public PlayerDataPvP PlayerData;
		public ulong TickCounter { get; private set; } // Big enough for years

		public bool IsClientRegistered => _isClientRegistered; // Is this instance a client
        public bool IsServerRegistered => _isServerRegistered; // Is this instance a server

        public bool isDebug = true; // Switch debug mode

		public static TextHudModule HudModule = new TextHudModule();

		#region ingame overrides

		/*
		 * Ingame Init
		 * 
		 * Main ingame initialization override
		 */
		public override void Init(MyObjectBuilder_SessionComponent sessionComponent)
		{
			base.Init(sessionComponent);

			// Set TickCounter always to zero at startup
			TickCounter = 0;
			 if (MyAPIGateway.Utilities == null) { MyAPIGateway.Utilities = MyAPIUtilities.Static; }
		}

		/*
		 * BeforeStart
		 * 
		 * Init networking
		 */
		public override void BeforeStart()
		{
			base.BeforeStart();
			HudModule.Init();
			// Register network handling
		   MyAPIGateway.Multiplayer.RegisterSecureMessageHandler(8544, MessageHandler);
		}

		private void MessageHandler(ushort handlerId, byte[] message, ulong steamId, bool isServer)
		{

			try
			{
				if (HudModule == null)
				{
					HudModule = new TextHudModule();
					HudModule.Init();
				}
				if (!HudModule.HudInit)
				{
					HudModule.Init();
				}
				var data = MyAPIGateway.Utilities.SerializeFromBinary<object>(message);
				switch (data)
                {
                    case ChatStatus status:
                        if (status.ChatEnabled)
                        {
                            HudModule.SetStatus(true);
                            HudModule.SetInfo(true);
                        }
                        else
                        {
                            HudModule.SetStatus(true);
                            HudModule.SetInfo(false);
                        }

						break;
                    case PlayerDataPvP playerData:
                        PlayerData = playerData;
                        break;
                    case PvPArea areaData:
                        if (PlayerData != null)
                        {

                        }
                        break;
                }





			}
			catch (Exception e)
			{
				MyLog.Default.WriteLineAndConsole($"{e.Message}\n{e.StackTrace}");

				if (MyAPIGateway.Session?.Player != null)
					MyAPIGateway.Utilities.ShowNotification($"[ ERROR: {GetType().FullName}: {e.Message} | Send SpaceEngineers.Log to mod author ]", 10000, MyFontEnum.Red);
			}
			
		}
		/*
		 * UpdateBeforeSimulation
		 * 
		 * UpdateBeforeSimulation override
		 */
		public override void UpdateBeforeSimulation()
		{
			// Init Block
			try
			{
				// Check if Instance exists
	   

				// This needs to wait until the MyAPIGateway.Session.Player is created, as running on a Dedicated server can cause issues.
				// It would be nicer to just read a property that indicates this is a dedicated server, and simply return.
				if (!_isInitialized && MyAPIGateway.Session != null && MyAPIGateway.Session.Player != null)
				{
					if (MyAPIGateway.Session.OnlineMode.Equals(MyOnlineModeEnum.OFFLINE)) // pretend single player instance is also server.
					{
						InitServer();
					}

					if (!MyAPIGateway.Session.OnlineMode.Equals(MyOnlineModeEnum.OFFLINE) && MyAPIGateway.Multiplayer.IsServer && !MyAPIGateway.Utilities.IsDedicated)
					{
						InitServer();
					}

					InitClient();
				}

				// Dedicated Server.
				if (!_isInitialized && MyAPIGateway.Utilities != null && MyAPIGateway.Multiplayer != null
					&& MyAPIGateway.Session != null && MyAPIGateway.Utilities.IsDedicated && MyAPIGateway.Multiplayer.IsServer)
				{
					InitServer();
					return;
				}

				//TODO: Why not before everything else?
				base.UpdateBeforeSimulation();
			}
			catch (Exception ex)
			{

			}
		}


		/*
		 * UpdateAfterSimulation
		 * 
		 * UpdateAfterSimulation override
		 */
		public override void UpdateAfterSimulation()
		{
			TickCounter += 1;
			//i dont do anything with this
		}

		public override void UpdatingStopped()
		{
			MyAPIGateway.Multiplayer.UnregisterSecureMessageHandler(8544, MessageHandler);
		}


		private void InitClient()
		{
			_isInitialized = true; // Set this first to block any other calls from UpdateAfterSimulation().
			_isClientRegistered = true;
		  //  ClientLogger.Init("CrunchChat_Client.Log", false, 0); // comment this out if logging is not required for the Client. "AppData\Roaming\SpaceEngineers\Storage"
		   // ClientLogger.WriteStart("CrunchChat Client Log Started");

		}

		private void InitServer()
		{
			try
			{
				_isInitialized = true; // Set this first to block any other calls from UpdateAfterSimulation().
				_isServerRegistered = true;
			 //   ServerLogger.Init("CrunchChat.Log", false, 0); // comment this out if logging is not required for the Server.
			 //   ServerLogger.WriteStart("CrunchChat Server Log Started");
			 //  ServerLogger.WriteInfo("CrunchChat Server Version {0}", Mod_Config.modVersion.ToString());
			  //  ServerLogger.WriteInfo("CrunchChat Communiction Server Version {0}", Mod_Config.ModCommunicationVersion);
			  //  if (ServerLogger.IsActive)
			   //     VRage.Utils.MyLog.Default.WriteLine(string.Format("##Mod## LST Server Logging File: {0}", ServerLogger.LogFile));



			}
			catch (Exception e)
			{
			 //   CrunchChat.Instance.ServerLogger.WriteException(e, "Core::InitServer");
			}
		}

		#endregion


		
	}
}
