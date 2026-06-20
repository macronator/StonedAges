using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Sockets;

namespace StonedAges;

public static class PacketHandler
{
    public static void Handle(byte[] packet, Socket clientSocket, GameState gs)
    {
        BitConverter.ToUInt16(packet, 0);
        switch (BitConverter.ToUInt16(packet, 2))
        {
            case 4:
                {
                    LocationPacket item = new LocationPacket(packet);
                    gs.locationPackets.Add(item);
                    break;
                }
            case 5:
                {
                    PlayerIDPacket playerIDPacket = new PlayerIDPacket(packet);
                    gs.ClientID = playerIDPacket.ID;
                    break;
                }
            case 7:
                {
                    EntityMapInfoPacket entityMapInfoPacket = new EntityMapInfoPacket(packet);
                    gs.PortToPlayer(entityMapInfoPacket.Number, entityMapInfoPacket.X, entityMapInfoPacket.Y);
                    break;
                }
            case 8:
                {
                    GroupRequestPacket groupRequestPacket = new GroupRequestPacket(packet);
                    gs.GroupRequest(groupRequestPacket.Name, groupRequestPacket.Type);
                    break;
                }
            case 12:
                {
                    BodyMovementPacket item10 = new BodyMovementPacket(packet);
                    gs.bodyMovementPackets.Add(item10);
                    break;
                }
            case 14:
                {
                    RemoveEntityPacket item9 = new RemoveEntityPacket(packet);
                    gs.removeEntityPackets.Add(item9);
                    break;
                }
            case 20:
                {
                    MessagePacket item8 = new MessagePacket(packet);
                    gs.messagePackets.Add(item8);
                    break;
                }
            case 21:
                {
                    SysMessagePacket sysMessagePacket = new SysMessagePacket(packet);
                    gs.SystemMessageP(sysMessagePacket.MsgType, sysMessagePacket.Message);
                    break;
                }
            case 28:
                {
                    DisplayProfilePacket item7 = new DisplayProfilePacket(packet);
                    gs.displayProfilePackets.Add(item7);
                    break;
                }
            case 30:
                {
                    UserListPacket userListPacket = new UserListPacket(packet);
                    GameState.UpdateUserList(userListPacket.List);
                    break;
                }
            case 33:
                {
                    DisplayPlayerPacket item6 = new DisplayPlayerPacket(packet);
                    gs.displayPlayerPackets.Add(item6);
                    break;
                }
            case 34:
                {
                    DisplayEntitiesPacket item5 = new DisplayEntitiesPacket(packet);
                    gs.displayEntitiesPackets.Add(item5);
                    break;
                }
            case 35:
                {
                    SpellAnimationPacket item4 = new SpellAnimationPacket(packet);
                    gs.spellAnimationPackets.Add(item4);
                    break;
                }
            case 36:
                {
                    ProjectilePacket item3 = new ProjectilePacket(packet);
                    gs.projectilePackets.Add(item3);
                    break;
                }
            case 39:
                {
                    DisplayBoardPacket item2 = new DisplayBoardPacket(packet);
                    gs.displayBoardPackets.Add(item2);
                    break;
                }
            case 100:
                {
                    JsonVersPacket jsonVersPacket = new JsonVersPacket(packet);
                    bool flag = true;
                    foreach (KeyValuePair<string, string> item11 in jsonVersPacket.List)
                    {
                        foreach (KeyValuePair<string, DateTime> item12 in GameWindow.JsonVersTime)
                        {
                            if (item11.Key == item12.Key)
                            {
                                if (item11.Value != item12.Value.ToString())
                                {
                                    flag = false;
                                }
                                break;
                            }
                        }
                    }
                    if (!flag)
                    {
                        Console.WriteLine("Starting patcher...");
                        Environment.Exit(1);
                    }
                    break;
                }
            case 102:
                {
                    ReceivedClientVersionPacket receivedClientVersionPacket = new ReceivedClientVersionPacket(packet);
                    DateTime dateTime = DateTime.ParseExact(receivedClientVersionPacket.Version, "MM.dd.yy-HH:mm:ss", CultureInfo.InvariantCulture);
                    if (GameWindow._clientVersion != dateTime)
                    {
                        File.WriteAllText("Warning.txt", "Your client is not up to date, please download the newest version at http://da-items.space/downloads.htm");
                        Process.Start("notepad.exe", "Warning.txt");
                        File.Delete("Warning.txt");
                        Environment.Exit(1);
                    }
                    break;
                }
        }
    }
}
