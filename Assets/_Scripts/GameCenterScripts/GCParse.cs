using System;
using ProtoBuf;
using UnityEngine;
using System.IO;

public class GCParse
{
	public static bool CmdParse(uint cmdid,byte[] body)
	{	
		switch ((Const.CMD)cmdid)
		{
		case Const.CMD.GC2C_REPLY_ENTER_GC:
			{
				var data = Serializer.Deserialize<CmdMessage.RePly_Enter_GameCenter>(new MemoryStream(body));
				if (data.error == (int)Const.ERROR_CODE.ERROR_OK)
				{
					Debug.Log(string.Format("进入游戏中心成功! user_id:{0} user_name:{1} money:{2} game_list:{1}",
						data.user_info.user_id,data.user_info.user_name,data.user_info.money));

					PublicData.Instance.userID = data.user_info.user_id;
					PublicData.Instance.userName =data.user_info.user_name;
					PublicData.Instance.userMoney = data.user_info.money;
					for (int i = 0; i < data.game_list.Count; i++)
					{
						Debug.Log(string.Format("游戏{0} 类型:{1} 服务器IP:{2} 端口:{3}",i,data.game_list[i].game_type, data.game_list[i].game_ip, data.game_list[i].game_port));
					}
					//显示玩家信息
					GCInit.userIDText.Invoke(GCInit.SetText,new object[] {GCInit.userIDText,"玩家ID"});
					GCInit.userNameText.Invoke(GCInit.SetText,new object[] {GCInit.userNameText,"玩家昵称"});
					GCInit.moneyText.Invoke(GCInit.SetText,new object[] {GCInit.moneyText,"玩家金币"});
				}
				else
				{
					Debug.Log("进入游戏中心失败!");
				}
			}
			break;
		}
		return true;
	}
}

