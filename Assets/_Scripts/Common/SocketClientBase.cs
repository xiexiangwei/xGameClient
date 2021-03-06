﻿using ProtoBuf;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class SocketClientBase
{
    private string ip;
    private int port;
    private AsyncTcpSession client;
    private List<byte> msgBuffer = new List<byte>();

    /// <summary>
    /// 建立连接
    /// </summary>
    public void Start(string _ip,int _port)
    {
        ip = _ip;
        port = _port;
        client = new AsyncTcpSession(new IPEndPoint(IPAddress.Parse(ip), port));
        client.Connected += Connected;
        client.DataReceived += DataReceived;
        client.Error += Error;
        client.Closed += Closed;
        client.Connect();
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="cmd">消息ID</param>
    /// <param name="data">消息内容</param>
    public void SendCmd(Const.CMD cmd, byte[] data)
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
    /// <summary>
    /// 关闭连接
    /// </summary>
    public void Close()
    {
        if (client != null)
        {
            if (client.IsConnected)
            {
                client.Close();
                client = null;
            }
        }
    }
    /// <summary>
    /// 连接成功事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Connected(object sender, EventArgs e)
    {
        OnConnected();
        //连接成功,请求进入游戏中心
        //CmdMessage.Request_Enter_GameCenter req = new CmdMessage.Request_Enter_GameCenter();
        //req.account_id = PublicData.Instance.accountID;
        //req.token = PublicData.Instance.userToken;
        ////序列化操作
        //MemoryStream ms = new MemoryStream();
        //Serializer.Serialize(ms, req);
        //SendCmd(Const.CMD.C2GC_REQUEST_ENTER_GC, ms.ToArray());
    }
    /// <summary>
    /// socket连接成功虚函数
    /// </summary>
    protected virtual void OnConnected() { }

    /// <summary>
    /// 接受消息事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="msg"></param>
    private void DataReceived(object sender, DataEventArgs msg)
    {
        //消息结构
        //包头=包体+包尾的长度+消息ID
        //消息总长度=包头(4)+包体+包尾的长度(2)
        msgBuffer.AddRange(msg.Data.CloneRange(msg.Offset, msg.Length));
        var msgArray = msgBuffer.ToArray();
        var msgLen = msgArray.Length;//消息总长度
        while (msgLen >= 4)
        {
            var len = BitConverter.ToUInt16(msgArray, 0);//包体+包尾的长度
            var cmdid = BitConverter.ToUInt16(msgArray, 2);//消息ID
            if (msgLen < len + 4)
            {
                break;
            }
            var body = msgBuffer.GetRange(4, len - 2).ToArray();
            msgBuffer.RemoveRange(0, len + 4);
            //处理这条消息
            CmdParse(cmdid, body);
            msgLen = msgBuffer.ToArray().Length;
        }
    }
    /// <summary>
    /// 消息处理虚函数
    /// </summary>
    /// <param name="cmdid"></param>
    /// <param name="data"></param>
    protected virtual void CmdParse(uint cmdid,byte [] data) { }


    /// <summary>
    /// 连接出错事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
    {
        Debug.Log(e.Exception.Message.ToString());
    }
    /// <summary>
    /// 连接断开事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Closed(object sender, EventArgs e)
    {
        Debug.Log("Closed()");
        if (client != null)
        {
            if (client.IsConnected)
            {
                client.Close();
            }
            client = null;
        }
    }
}

