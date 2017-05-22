using System;
using ProtoBuf;
using UnityEngine;
using System.IO;

public class SMParse
{
	public static bool CmdParse(uint cmdid,byte[] body)
	{	
		switch ((Const.CMD)cmdid)
		{
		//请求网关返回
		case Const.CMD.SM2C_GET_LOGINGATE_REPLY:
			{
				var data = Serializer.Deserialize<CmdMessage.RePly_Get_LoginGateInfo>(new MemoryStream(body));
				if (data.error == (int)Const.ERROR_CODE.ERROR_OK)
				{
					Debug.Log(string.Format("RePly_Get_LoginGateInfo  error:{0} ip:{1} port:{2}", data.error, data.ip, data.port));
					//连接登陆网关服务器
					LoginInit.lgClient = new LGServer(data.ip, (int)data.port);
					LoginInit.lgClient.Start();
				}
				else
				{
					Debug.Log("没有可用网关!");
				}
			}
			break;
		case Const.CMD.SM2C_GET_GAMECENTER_REPLY:
			{
				var data = Serializer.Deserialize<CmdMessage.Reply_Get_GameCenter>(new MemoryStream(body));
				if (data.error == (uint)Const.ERROR_CODE.ERROR_OK)
				{
					Debug.Log(string.Format("请求游戏中心返回  IP:{0} Port:{1}", data.gamecenter_ip, data.gamecenter_port));
					PublicData.Instance.gcIP = data.gamecenter_ip;
					PublicData.Instance.gcPort = data.gamecenter_port;
					LoginInit.loadGCScene = true;
				}
				else
				{
					Debug.Log(string.Format("获取游戏中心信息失败!  Error:{0}", data.error));
				}
			}
			break;
		}
		return true;
	}
}

