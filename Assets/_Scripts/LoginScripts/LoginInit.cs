using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperSocket.ClientEngine;
using System.Net;
using System;
using ProtoBuf;
using System.IO;
using UnityEngine.UI;

public class LoginInit : MonoBehaviour
{

    //管理服务器客户端
    private static AsyncTcpSession smClient;
    private static AsyncTcpSession lgClient;

    //登录网关IP
    private string logingateIP;
    //登录网关端口
    private int logingatePort;

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
    public static void Send2LoginGate(Const.CMD cmd, byte[] data)
    {
       SendCmd(lgClient,cmd,data);
    }
    public static void Send2SM(Const.CMD cmd, byte[] data)
    {
       SendCmd(smClient,cmd,data);
    }
    void Start()
    {
        Debug.Log("LoginInit Start()");
        smClient = new AsyncTcpSession(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1111));
        smClient.Connected += SM_Connected;
        smClient.DataReceived += SM_DataReceived;
        smClient.Error += SM_Error;
        smClient.Closed += SM_Closed;
        smClient.Connect();
    }
    private void SM_Connected(object sender, EventArgs e)
    {
        Debug.Log("SM_Connected()");
        Send2SM(Const.CMD.C2SM_GET_LOGINGATE, new byte[] { });
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

    private void SM_DataReceived(object sender, DataEventArgs cmd)
    {
        Debug.Log("SM_DataReceived()");
        var len = BitConverter.ToUInt16(cmd.Data, 0);
        var cmdid = BitConverter.ToUInt16(cmd.Data, 2);
        if (cmdid == (int)Const.CMD.SM2C_GET_LOGINGATE_REPLY)
        {
            byte[] body = new byte[len - 2];
            Buffer.BlockCopy(cmd.Data, 4, body, 0, len - 2);

            var data = Serializer.Deserialize<CmdMessage.RePly_Get_LoginGateInfo>(new MemoryStream(body));
            if (data.error == (int)Const.ERROR_CODE.ERROR_OK)
            {
                Debug.Log(string.Format("RePly_Get_LoginGateInfo  error:{0} ip:{1} port:{2}", data.error, data.ip, data.port));
                logingateIP = data.ip;
                logingatePort = (int)data.port;
                //主动断开服务器管理连接
                smClient.Close();
                smClient = null;
                //连接登陆网关服务器
                lgClient = new AsyncTcpSession(new IPEndPoint(IPAddress.Parse(logingateIP), logingatePort));
                lgClient.Connected += LG_Connected;
                lgClient.DataReceived += LG_DataReceived;
                lgClient.Error += LG_Error;
                lgClient.Closed += LG_Closed;
                lgClient.Connect();

            }
            else
            {
                Debug.Log("没有可用网关!");
            }

        }
    }
    private void LG_Connected(object sender, EventArgs e)
    {
        Debug.Log("LG_Connected()");
    }
    private void LG_Closed(object sender, EventArgs e)
    {
        Debug.Log("LG_Closed()");
        if (lgClient != null)
        {
            if (lgClient.IsConnected)
            {
                lgClient.Close();
            }
            lgClient = null;
        }
    }

    private void LG_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
    {
        Debug.Log(string.Format("LG_Error() {0}", e.Exception));
    }

    private void LG_DataReceived(object sender, DataEventArgs cmd)
    {
        var len = BitConverter.ToUInt16(cmd.Data, 0);
        var cmdid = BitConverter.ToUInt16(cmd.Data, 2);
        byte[] body = new byte[len - 2];
        Buffer.BlockCopy(cmd.Data, 4, body, 0, len - 2);

        Debug.Log(string.Format("LG_DataReceived() cmdid:{0}",cmdid));
        if (cmdid == (int)Const.CMD.LG2C_READY_TO_LOGIN)
        {
            var data = Serializer.Deserialize<CmdMessage.Reply_Connect_Logingate>(new MemoryStream(body));
            if (data.error == (int)Const.ERROR_CODE.ERROR_OK)
            {
                Debug.Log("登录服务器准备完毕，可以请求登录或者注册!");
            }
            else
            {
                Debug.Log("没有可用的登录服务器!");
            }
        }
        else if (cmdid==(int)Const.CMD.LG2C_LOGIN_RESULT)
        {
            var data = Serializer.Deserialize<CmdMessage.Reply_Login>(new MemoryStream(body));
            if (data.error == (int)Const.ERROR_CODE.ERROR_OK)
            {
                Debug.Log(string.Format("登录成功！ account_id:{0} token:{1}", data.account_id, data.token));
            }
            else
            {
                if(data.error==(int)Const.ERROR_CODE.ERROR_SERVER)
                {
                     Debug.Log("游戏服务器逻辑出错!");
                }
                else if(data.error==(int)Const.ERROR_CODE.ERROR_ACCOUNT_NOT_EXISTS)
                {
                     Debug.Log("帐号不存在!");
                }
                else if(data.error==(int)Const.ERROR_CODE.ERROR_ACCOUNT_PWD_ERROR)
                {
                     Debug.Log("密码错误!");
                }
            }
        }
    }
 
    private void OnDestroy()
    {
        Debug.Log("OnDestroy");

        //断开管理服务器连接
        if (smClient != null)
        {
            if (smClient.IsConnected)
            {
                smClient.Close();
            }
            smClient = null;
        }
        //断开登录网关连接
        if (lgClient != null)
        {
            if (lgClient.IsConnected)
            {
                lgClient.Close();
                Debug.Log(" lgClient.Close()");
            }
            lgClient = null;
        }
    }
}
