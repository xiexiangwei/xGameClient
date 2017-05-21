using SuperSocket.ClientEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using System;
using ProtoBuf;
using System.IO;

public class GCInit : MonoBehaviour {

    private static AsyncTcpSession gcClient;//游戏中心客户端
    public Text userIDText;//玩家ID标签
    public Text userNameText;//玩家昵称标签
    public Text moneyText;//玩家金币标签

    private bool showUserInfo = false;

    void Start ()
    {   
        //连接游戏中心
        gcClient = new AsyncTcpSession(new IPEndPoint(IPAddress.Parse(PublicData.Instance.gcIP), (int)PublicData.Instance.gcPort));
        gcClient.Connected += GC_Connected;
        gcClient.DataReceived += GC_DataReceived;
        gcClient.Error += GC_Error;
        gcClient.Closed += GC_Closed;
        gcClient.Connect();
    }
    private void GC_Connected(object sender, EventArgs e)
    {
        Debug.Log("GC_Connected()");
        //连接成功,请求进入游戏中心
        CmdMessage.Request_Enter_GameCenter req = new CmdMessage.Request_Enter_GameCenter();
        req.account_id = PublicData.Instance.accountID;
        req.token = PublicData.Instance.userToken;
        Debug.Log(string.Format("请求进入游戏中心,account_id:{0} token:{1}", req.account_id, req.token));
        //序列化操作
        MemoryStream ms = new MemoryStream();
        Serializer.Serialize(ms, req);
        Send2GC(Const.CMD.C2GC_REQUEST_ENTER_GC, ms.ToArray());
    }

    private void GC_DataReceived(object sender, DataEventArgs cmd)
    {
        Debug.Log("GC_DataReceived()");
        var len = BitConverter.ToUInt16(cmd.Data, 0);
        var cmdid = BitConverter.ToUInt16(cmd.Data, 2);
        byte[] body = new byte[len - 2];
        Buffer.BlockCopy(cmd.Data, 4, body, 0, len - 2);

        Debug.Log(string.Format("GC_DataReceived() cmdid:{0}", cmdid));
        if (cmdid == (int)Const.CMD.GC2C_REPLY_ENTER_GC)
        {
            var data = Serializer.Deserialize<CmdMessage.RePly_Enter_GameCenter>(new MemoryStream(body));
            if (data.error == (int)Const.ERROR_CODE.ERROR_OK)
            {
                Debug.Log(string.Format("进入游戏中心成功! user_id:{0} user_name:{1} money:{2} game_list:{1}",
                    data.user_info.user_id,data.user_info.user_name,data.user_info.money));

                PublicData.Instance.userID = data.user_info.user_id;
                PublicData.Instance.userName =data.user_info.user_name;
                PublicData.Instance.userMoney = data.user_info.money;
                showUserInfo = true;

                for (int i = 0; i < data.game_list.Count; i++)
                {
                    Debug.Log(string.Format("游戏{0} 类型:{1} 服务器IP:{2} 端口:{3}",i,data.game_list[i].game_type, data.game_list[i].game_ip, data.game_list[i].game_port));
                }
            }
            else
            {
                Debug.Log("进入游戏中心失败!");
            }
        }

    }
    private void GC_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
    {
        Debug.Log("GC_Error()");
    }

    private void GC_Closed(object sender, EventArgs e)
    {
        Debug.Log("GC_Closed()");
    }

    private static void SendCmd(AsyncTcpSession client, Const.CMD cmd, byte[] data)
    {
        var lenArray = BitConverter.GetBytes(data.Length + 2);
        var cmdArray = BitConverter.GetBytes((int)cmd);
        byte[] head = new byte[] { lenArray[0], lenArray[1], cmdArray[0], cmdArray[1] };
        var sendData = new byte[6 + data.Length];//6=包头(4)+包尾(2)
        //包头
        Buffer.BlockCopy(head, 0, sendData, 0, 4);
        //包体
        Buffer.BlockCopy(data, 0, sendData, 4 * sizeof(byte), data.Length);
        //包尾
        byte[] tail = new byte[] { 0x0, 0x0 };
        Buffer.BlockCopy(tail, 0, sendData, (4 + data.Length) * sizeof(byte), 2);

        client.Send(sendData, 0, sendData.Length);
    }
    public static void Send2GC(Const.CMD cmd, byte[] data)
    {
        SendCmd(gcClient, cmd, data);
    }

    private void Update()
    {
        if (showUserInfo)
        {
            showUserInfo = false;
            userIDText.text = string.Format("ID:{0}", PublicData.Instance.userID);
            userNameText.text = PublicData.Instance.userName;
            moneyText.text = string.Format("金币:{0}", PublicData.Instance.userMoney);
        }
    }

    private void OnDestroy()
    {
        Debug.Log("OnDestroy");

        //断开游戏中心连接
        if (gcClient != null)
        {
            if (gcClient.IsConnected)
            {
                gcClient.Close();
            }
            gcClient = null;
        }
      
    }


}
