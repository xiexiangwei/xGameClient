using SuperSocket.ClientEngine;
using System.Net;
using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// LG server.登录网关类
/// </summary>
public class LGServer
{
	private string gm_ip;//登录网关类地址
	private int gm_port;//登录网关类端口
	private static AsyncTcpSession lgClient;//登录网关类客户端
	private List<byte> msgBuffer = new List<byte>();//消息缓存器
	/// <summary>
	/// 构造函数
	/// </summary>
	/// <param name="ip"></param>
	/// <param name="port"></param>
	public LGServer(string ip, int port)
	{
		gm_ip = ip;
		gm_port = port;
	}
	/// <summary>
	/// 建立连接
	/// </summary>
	public void Start()
	{
		lgClient = new AsyncTcpSession(new IPEndPoint(IPAddress.Parse(gm_ip), gm_port));
		lgClient.Connected += Connected;
		lgClient.DataReceived += DataReceived;
		lgClient.Error += Error;
		lgClient.Closed += Closed;
		lgClient.Connect();
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

		lgClient.Send(sendData, 0, sendData.Length);
	}
	/// <summary>
	/// 关闭连接
	/// </summary>
	public void Close()
	{
		if (lgClient != null)
		{
			if (lgClient.IsConnected)
			{
				lgClient.Close();
				lgClient = null;
			}
		}
	}
	/// <summary>
	/// 连接成功
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Connected(object sender, EventArgs e)
	{
		Debug.Log("登录网关连接成功");
	}
	/// <summary>
	/// 接受消息
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="msg"></param>
	private void DataReceived(object sender, DataEventArgs msg)
	{
		//消息结构
		//包头=包体+包尾的长度+消息ID
		//消息总长度=包头(4)+包体+包尾的长度(2)
		msgBuffer.AddRange(msg.Data.CloneRange(msg.Offset,msg.Length));
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
			LGParse.CmdParse(cmdid,body);
			msgLen = msgBuffer.ToArray().Length;
		}
	}
	/// <summary>
	/// 连接出错
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
	{
		Debug.Log(e.Exception.Message.ToString());
	}
	/// <summary>
	/// 连接断开
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Closed(object sender, EventArgs e)
	{
		Debug.Log("Closed()");
		if (lgClient != null)
		{
			if (lgClient.IsConnected)
			{
				lgClient.Close();
			}
			lgClient = null;
		}
	}
}
