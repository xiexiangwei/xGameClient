using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperSocket.ClientEngine;
using System.Net;
using System;
using ProtoBuf;
using System.IO;

public class Login : MonoBehaviour {

    //管理服务器客户端
    private static AsyncTcpSession smClient;
    public void SendCmd(AsyncTcpSession client,Const.CMD cmd,byte[] data)
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
    void Start ()
    {
        Debug.Log("Start()");
        smClient = new AsyncTcpSession(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1111));
        smClient.Connected += SM_Connected;
        smClient.DataReceived += SMt_DataReceived;
        smClient.Error += SM_Error;
        smClient.Closed += SM_Closed;
        smClient.Connect();
    }

    private void SM_Closed(object sender, EventArgs e)
    {
        Debug.Log("SM_Closed()");
        if (smClient != null)
        {
            if (smClient.IsConnected)
            {
                smClient.Close();
            }
            smClient = null;
        }
    }

    private void SM_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
    {
        Debug.Log(e.Exception.Message.ToString());
    }

    private void SMt_DataReceived(object sender, DataEventArgs cmd)
    {
        Debug.Log("SMt_DataReceived()");
        var len = BitConverter.ToUInt16(cmd.Data, 0);
        var cmdid = BitConverter.ToUInt16(cmd.Data, 2);
        if (cmdid ==(int)Const.CMD.SM2C_GET_LOGINGATE_REPLY)
        {
            byte[] body = new byte[len - 2];
            Buffer.BlockCopy(cmd.Data, 4, body, 0, len - 2);

            var data = Serializer.Deserialize<CmdMessage.RePly_Get_LoginGateInfo>(new MemoryStream(body));
            if(data.error==(int)Const.ERROR_CODE.ERROR_OK)
            {
                Debug.Log(string.Format("error:{0} ip:{1} port:{2}", data.error, data.ip, data.port));
            }
            else
            {
                Debug.Log("没有可用网关!");
            }

        }
    }

    private void SM_Connected(object sender, EventArgs e)
    {
        Debug.Log("SM_Connected()");
        SendCmd(smClient, Const.CMD.C2SM_GET_LOGINGATE, new byte[] { });
    }

    void Update () 
    {
        Debug.Log("Update()");
    }

    public void OnLoginClick()
    {
        Debug.Log("登录成功");
    }
    public void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        if (smClient != null)
        {
            if (smClient.IsConnected)
            {
                smClient.Close();
            }
            smClient = null;
        }
    }
}
