﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

#if !NoAttributes
using ScriptCoreLib;
#endif

namespace FlashTowerDefense.Shared
{


    public partial class SharedClass1
    {


#if !NoAttributes
        [Script]
#endif
        [CompilerGenerated]
        public enum Messages
        {
            None = 100,

            Ping,
            Pong,

            UserJoined,
            ToUserJoinedReply,  // client->server
            UserJoinedReply,    // server->client

            UserLeft,

            UserEnterMachineGun,
            UserExitMachineGun,

            UserStartMachineGun,
            UserStopMachineGun,

            UserTeleportTo,
            UserWalkTo,
            UserFiredShotgun,

            EnterMachineGun,
            ExitMachineGun,

            StartMachineGun,
            StopMachineGun,

            TeleportTo,
            WalkTo,
            FiredShotgun,

            ServerMessage,
            ServerRandomNumbers,

            ReadyForServerRandomNumbers,
            CancelServerRandomNumbers,

            AddDamageFromDirection,
            UserAddDamageFromDirection,


            // for others
            TakeBox,
            UserTakeBox,


            ShowBulletsFlying,
            UserShowBulletsFlying,
        }

        #region RemoteMessages

#if !NoAttributes
        [Script]
#endif
        [CompilerGenerated]
        public sealed class RemoteMessages : IMessages
        {
            public Action<SendArguments> Send;
            #region SendArguments

#if !NoAttributes
            [Script]
#endif
            [CompilerGenerated]
            public sealed class SendArguments
            {
                public Messages i;
                public object[] args;
            }
            #endregion

            public void TeleportTo(int x, int y)
        {
            Send(new SendArguments { i = Messages.TeleportTo, args = new object[] { x, y } })
        }
            public void UserTeleportTo(int user, int x, int y)
        {
            Send(new SendArguments { i = Messages.UserTeleportTo, args = new object[] { user, x, y } })
        }
        }
        #endregion


        #region RemoteEvents

#if !NoAttributes
        [Script]
#endif
        [CompilerGenerated]
        public sealed class RemoteEvents
        {
            private readonly Dictionary<Messages, Action<DispatchHelper>> DispatchTable;
            #region DispatchHelper

#if !NoAttributes
            [Script]
#endif
            [CompilerGenerated]
            public sealed class DispatchHelper
            {
                public Converter<uint, int> GetInt32;
                public Converter<uint, double> GetDouble;
                public Converter<uint, string> GetString;
            }
            #endregion

            public bool Dispatch(Messages e, DispatchHelper h)
            {
                if (!DispatchTable.ContainsKey(e)) return false;
                DispatchTable[e](h);
                return true;
            }
            #region TeleportToArguments

#if !NoAttributes
            [Script]
#endif
            [CompilerGenerated]
            public sealed class TeleportToArguments
            {
                public int x;
                public int y;
            }
            #endregion

            public event Action<TeleportToArguments> TeleportTo;
            #region UserTeleportToArguments

#if !NoAttributes
            [Script]
#endif
            [CompilerGenerated]
            public sealed class UserTeleportToArguments
            {
                public int user;
                public int x;
                public int y;
            }
            #endregion

            public event Action<UserTeleportToArguments> UserTeleportTo;
            public RemoteEvents()
            {
                DispatchTable = new Dictionary<Messages, Action<DispatchHelper>>
                {
                    { Messages.TeleportTo, e => TeleportTo(new TeleportToArguments { x = e.GetInt32(0), y = e.GetInt32(1) }) },
                    { Messages.UserTeleportTo, e => UserTeleportTo(new UserTeleportToArguments { user = e.GetInt32(0), x = e.GetInt32(1), y = e.GetInt32(2) }) },
                }
                ;
            }
        }
        #endregion
    }
}
